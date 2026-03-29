#!/usr/bin/env node
/**
 * GeoNames veri indirme ve provinces.json / districts.json / neighborhoods.json olusturma scripti.
 *
 * Kaynak: https://download.geonames.org/export/dump/
 * - admin1CodesASCII.txt: Il/Eyalet/Bolge (admin1 divisions)
 * - admin2Codes.txt: Ilce/County (admin2 divisions)
 * - cities5000.zip: Nufusu 5000+ sehirler (admin2 olmayan ulkeler icin fallback)
 * - allCountries.zip: TUM GeoNames veritabani (~12M satir) — gercek koordinatlar + mahalleler
 * - alternateNamesV2.zip: Coklu dil cevirileri
 *
 * Strateji:
 *   1. admin1 → provinces
 *   2. admin2 → districts (170 ulke), cities5000 fallback (24 ulke)
 *   3. allCountries → gercek koordinat + population + timezone (admin1/admin2/cities icin)
 *   4. allCountries ADM3/ADM4 → neighborhoods (mahalle/koyu)
 *   5. alternateNames → coklu dil cevirileri
 *
 * Kullanim: node generate_locations.js
 */

const https = require("https");
const http = require("http");
const fs = require("fs");
const path = require("path");
const readline = require("readline");
const AdmZip = require("adm-zip");

const SCRIPT_DIR = __dirname;

const ADMIN1_URL = "https://download.geonames.org/export/dump/admin1CodesASCII.txt";
const ADMIN2_URL = "https://download.geonames.org/export/dump/admin2Codes.txt";
const CITIES_URL = "https://download.geonames.org/export/dump/cities5000.zip";
const ALLCOUNTRIES_URL = "https://download.geonames.org/export/dump/allCountries.zip";
const ALT_NAMES_URL = "https://download.geonames.org/export/dump/alternateNamesV2.zip";

const TARGET_LANGUAGES = new Set([
  "en","tr","de","fr","es","ar","ru","zh","ja","ko","pt","it","nl","pl","hi",
  "fa","ur","az","id","th","vi","uk","ro","el","he","sv","da","no","fi","cs",
  "hu","bg","hr","sr","sk","sl","lt","lv","et","ka","hy","ms","tl","sw","bn",
  "ta","te","mr","gu","pa"
]);

// Mahalle/koyu seviyesi: tum idari alt bolumler + tum yerlseim yerleri
const NEIGHBORHOOD_FEATURE_CODES = new Set(["ADM3", "ADM4", "PPL", "PPLL", "PPLX", "PPLA3", "PPLA4"]);

// ─── Helpers ───────────────────────────────────────────

function download(url, desc) {
  return new Promise((resolve, reject) => {
    const get = url.startsWith("https") ? https.get : http.get;
    process.stdout.write(`  Indiriliyor: ${desc}...\n`);
    get(url, { headers: { "User-Agent": "LivestockTrading-GeoSeeder/1.0" } }, (res) => {
      if (res.statusCode >= 300 && res.statusCode < 400 && res.headers.location) {
        return download(res.headers.location, desc).then(resolve).catch(reject);
      }
      if (res.statusCode !== 200) return reject(new Error(`HTTP ${res.statusCode} for ${url}`));
      const total = parseInt(res.headers["content-length"] || "0", 10);
      const chunks = [];
      let downloaded = 0;
      res.on("data", (chunk) => {
        chunks.push(chunk);
        downloaded += chunk.length;
        if (total > 0) {
          const pct = Math.floor((downloaded / total) * 100);
          process.stdout.write(`\r  ${desc}: ${pct}% (${Math.floor(downloaded/1024/1024)}MB / ${Math.floor(total/1024/1024)}MB)`);
        }
      });
      res.on("end", () => { process.stdout.write("\n"); resolve(Buffer.concat(chunks)); });
      res.on("error", reject);
    }).on("error", reject);
  });
}

function downloadToFile(url, filePath, desc) {
  return new Promise((resolve, reject) => {
    const get = url.startsWith("https") ? https.get : http.get;
    process.stdout.write(`  Indiriliyor (diske): ${desc}...\n`);
    get(url, { headers: { "User-Agent": "LivestockTrading-GeoSeeder/1.0" } }, (res) => {
      if (res.statusCode >= 300 && res.statusCode < 400 && res.headers.location) {
        return downloadToFile(res.headers.location, filePath, desc).then(resolve).catch(reject);
      }
      if (res.statusCode !== 200) return reject(new Error(`HTTP ${res.statusCode}`));
      const total = parseInt(res.headers["content-length"] || "0", 10);
      const ws = fs.createWriteStream(filePath);
      let downloaded = 0;
      res.on("data", (chunk) => {
        ws.write(chunk);
        downloaded += chunk.length;
        if (total > 0) {
          const pct = Math.floor((downloaded / total) * 100);
          process.stdout.write(`\r  ${desc}: ${pct}% (${Math.floor(downloaded/1024/1024)}MB / ${Math.floor(total/1024/1024)}MB)`);
        }
      });
      res.on("end", () => { ws.end(); process.stdout.write("\n"); resolve(); });
      res.on("error", (err) => { ws.end(); reject(err); });
    }).on("error", reject);
  });
}

function extractZipEntry(zipBuffer, entryNameContains) {
  const zip = new AdmZip(zipBuffer);
  const entries = [];
  for (const entry of zip.getEntries()) {
    if (entry.entryName.includes(entryNameContains) && !entry.isDirectory) {
      entries.push({ name: entry.entryName, data: entry.getData() });
    }
  }
  return entries;
}

/**
 * Buyuk ZIP dosyasini diske cikarip readline stream dondurur.
 * Return: { rl, tmpFile, cleanup }
 */
function extractZipToStream(zipPath, entryNameContains) {
  const tmpDir = path.join(SCRIPT_DIR, "_tmp_geonames");
  if (!fs.existsSync(tmpDir)) fs.mkdirSync(tmpDir);
  const zip = new AdmZip(zipPath);
  const entry = zip.getEntries().find(e => e.entryName.includes(entryNameContains) && !e.isDirectory);
  if (!entry) return null;
  const tmpFile = path.join(tmpDir, entry.entryName.replace(/\//g, "_"));
  console.log(`  ZIP'ten diske cikariliyor: ${entry.entryName}...`);
  zip.extractEntryTo(entry, tmpDir, false, true);
  const rl = readline.createInterface({ input: fs.createReadStream(tmpFile, { encoding: "utf8" }), crlfDelay: Infinity });
  const cleanup = () => { try { fs.unlinkSync(tmpFile); fs.rmdirSync(tmpDir); } catch {} };
  return { rl, tmpFile, cleanup };
}

// ─── Parsers ───────────────────────────────────────────

function loadCountryMapping() {
  const filePath = path.join(SCRIPT_DIR, "countries.json");
  const countries = JSON.parse(fs.readFileSync(filePath, "utf8"));
  const map = {};
  for (const c of countries) map[c.Code] = c.Id;
  return map;
}

function parseAdmin1(content, countryMap) {
  const lines = content.toString("utf8").split("\n");
  const provinces = [];
  for (const line of lines) {
    const parts = line.trim().split("\t");
    if (parts.length < 4) continue;
    const codeFull = parts[0];
    const name = parts[1];
    const geonameId = parseInt(parts[3], 10);
    const dotIdx = codeFull.indexOf(".");
    if (dotIdx < 0) continue;
    const cc = codeFull.substring(0, dotIdx);
    const adminCode = codeFull.substring(dotIdx + 1);
    const countryId = countryMap[cc];
    if (!countryId) continue;
    provinces.push({ countryCode: cc, countryId, adminCode, name, geonameId, codeFull });
  }
  return provinces;
}

function parseAdmin2(content, countryMap, admin1Lookup) {
  const lines = content.toString("utf8").split("\n");
  const districts = [];
  for (const line of lines) {
    const parts = line.trim().split("\t");
    if (parts.length < 4) continue;
    const codeFull = parts[0];
    const name = parts[1];
    const geonameId = parseInt(parts[3], 10);
    const firstDot = codeFull.indexOf(".");
    if (firstDot < 0) continue;
    const cc = codeFull.substring(0, firstDot);
    const rest = codeFull.substring(firstDot + 1);
    const secondDot = rest.indexOf(".");
    if (secondDot < 0) continue;
    const admin1Code = rest.substring(0, secondDot);
    const countryId = countryMap[cc];
    if (!countryId) continue;
    const admin1Key = `${cc}.${admin1Code}`;
    if (!admin1Lookup[admin1Key]) continue;
    districts.push({
      geonameId, name, countryCode: cc, admin1Key,
      admin2Code: rest.substring(secondDot + 1),
      population: 0, latitude: null, longitude: null, timezone: null,
      source: "admin2",
    });
  }
  return districts;
}

/** cities5000: sadece fallback ulkeler icin district verisi */
function parseCities(zipBuffer, countryMap, admin1Lookup) {
  const cities = [];
  const entries = extractZipEntry(zipBuffer, "cities5000");
  for (const entry of entries) {
    const lines = entry.data.toString("utf8").split("\n");
    for (const line of lines) {
      const parts = line.split("\t");
      if (parts.length < 18) continue;
      const geonameId = parseInt(parts[0], 10);
      const cityName = parts[1];
      const latitude = parseFloat(parts[4]) || null;
      const longitude = parseFloat(parts[5]) || null;
      const cc = parts[8];
      const admin1Code = parts[10];
      const population = parseInt(parts[14], 10) || 0;
      const timezone = parts[17] || null;
      const countryId = countryMap[cc];
      if (!countryId) continue;
      const admin1Key = `${cc}.${admin1Code}`;
      if (!admin1Lookup[admin1Key]) continue;
      cities.push({ geonameId, name: cityName, countryCode: cc, admin1Key, population, latitude, longitude, timezone });
    }
  }
  return cities;
}

/**
 * allCountries.txt tek geciste tara:
 * 1. targetGeonameIds icin: lat, lng, population, timezone (provinces + districts + cities)
 * 2. ADM3/ADM4 kayitlari: neighborhoods olarak topla
 *
 * @param {string} zipPath - _tmp_allcountries.zip yolu
 * @param {Set<number>} targetGeonameIds - koordinat istenen geonameId'ler
 * @param {Map<string, number>} admin2KeyToGeonameId - "CC.admin1.admin2" → admin2 geonameId (neighborhood baglama icin)
 * @param {Object} countryMap - CC → countryId
 * @param {Object} admin1Lookup - "CC.admin1" → province info
 */
function parseAllCountries(zipPath, targetGeonameIds, admin2KeyToGeonameId, countryMap, admin1Lookup) {
  console.log("  allCountries.txt parse ediliyor (~12M satir, tek gecis)...");
  const stream = extractZipToStream(zipPath, "allCountries");
  if (!stream) { console.log("  WARN: allCountries.txt bulunamadi!"); return Promise.resolve({ geoCoords: new Map(), rawNeighborhoods: [] }); }

  const geoCoords = new Map(); // geonameId → { lat, lng, population, timezone }
  const rawNeighborhoods = []; // ADM3/ADM4 kayitlari

  let lineCount = 0;
  return new Promise((resolve) => {
    stream.rl.on("line", (line) => {
      lineCount++;
      if (lineCount % 2_000_000 === 0) process.stdout.write(`\r  ${Math.floor(lineCount / 1_000_000)}M satir...`);
      const parts = line.split("\t");
      if (parts.length < 18) return;

      const geonameId = parseInt(parts[0], 10);
      const name = parts[1];
      const lat = parseFloat(parts[4]);
      const lng = parseFloat(parts[5]);
      const featureCode = parts[7];
      const cc = parts[8];
      const admin1Code = parts[10];
      const admin2Code = parts[11];
      const population = parseInt(parts[14], 10) || 0;
      const timezone = parts[17] || null;

      // 1. Koordinat lookup (provinces, districts, cities)
      if (targetGeonameIds.has(geonameId)) {
        geoCoords.set(geonameId, { lat, lng, population, timezone });
      }

      // 2. Neighborhood verisi (ADM3, ADM4)
      if (NEIGHBORHOOD_FEATURE_CODES.has(featureCode) && cc && admin1Code) {
        const countryId = countryMap[cc];
        if (!countryId) return;
        const admin1Key = `${cc}.${admin1Code}`;
        if (!admin1Lookup[admin1Key]) return;

        // admin2_code dolu ise admin2'ye bagla
        if (admin2Code && admin2Code.trim()) {
          const a2key = `${cc}.${admin1Code}.${admin2Code}`;
          const admin2GeoId = admin2KeyToGeonameId.get(a2key);
          if (admin2GeoId) {
            rawNeighborhoods.push({ geonameId, name, admin1Key, admin2GeonameId: admin2GeoId, lat, lng });
          }
        }
        // admin2_code bos ise (TR gibi): ilerde coğrafi yakınlık ile eşleştirilebilir, şimdilik atla
      }
    });

    stream.rl.on("close", () => {
      console.log(`\r  ${Math.floor(lineCount / 1_000_000)}M satir islendi. ${geoCoords.size} koordinat, ${rawNeighborhoods.length} mahalle/koyu bulundu.`);
      stream.cleanup();
      resolve({ geoCoords, rawNeighborhoods });
    });
  });
}

/** alternateNamesV2 parse et (buyuk dosya, satir satir) */
function parseAlternateNames(zipPath, targetGeonameIds) {
  console.log("  alternateNamesV2 parse ediliyor...");
  const stream = extractZipToStream(zipPath, "alternateNamesV2");
  if (!stream) { console.log("  WARN: alternateNamesV2.txt bulunamadi!"); return Promise.resolve({}); }

  const translations = {};
  let lineCount = 0;
  return new Promise((resolve) => {
    stream.rl.on("line", (line) => {
      lineCount++;
      if (lineCount % 5_000_000 === 0) process.stdout.write(`\r  ${Math.floor(lineCount / 1_000_000)}M satir...`);
      const parts = line.split("\t");
      if (parts.length < 4) return;
      const gid = parseInt(parts[1], 10);
      if (!targetGeonameIds.has(gid)) return;
      const lang = parts[2];
      if (!TARGET_LANGUAGES.has(lang)) return;
      const altName = parts[3];
      const isPreferred = parts[4] === "1";
      const isHistoric = parts.length > 7 && parts[7] === "1";
      if (isHistoric) return;
      if (!translations[gid]) translations[gid] = {};
      if (!translations[gid][lang] || isPreferred) translations[gid][lang] = altName;
    });
    stream.rl.on("close", () => {
      console.log(`\r  ${Math.floor(lineCount / 1_000_000)}M satir islendi. ${Object.keys(translations).length} kayit icin ceviri bulundu.`);
      stream.cleanup();
      resolve(translations);
    });
  });
}

// ─── Builders ──────────────────────────────────────────

function buildProvincesJson(rawProvinces, translations, geoCoords) {
  rawProvinces.sort((a, b) => a.countryId - b.countryId || a.name.localeCompare(b.name));
  const result = [];
  const countrySeq = {};
  for (const p of rawProvinces) {
    const cid = p.countryId;
    countrySeq[cid] = (countrySeq[cid] || 0) + 1;
    const seq = countrySeq[cid];
    const provinceId = cid * 10000 + seq;
    const nameTrans = translations[p.geonameId] || null;
    const coords = geoCoords.get(p.geonameId);
    result.push({
      Id: provinceId,
      CountryId: cid,
      Name: p.name,
      Code: p.adminCode,
      NameTranslations: nameTrans ? JSON.stringify(nameTrans) : null,
      GeoNameId: p.geonameId,
      Latitude: coords?.lat ?? null,
      Longitude: coords?.lng ?? null,
      Population: coords?.population ?? null,
      Timezone: coords?.timezone ?? null,
      SortOrder: seq,
    });
  }
  return result;
}

function buildDistrictsJson(admin2Districts, citiesFallback, provincesJson, admin1Lookup, translations, geoCoords) {
  const geoToProvinceId = {};
  for (const pj of provincesJson) geoToProvinceId[pj.GeoNameId] = pj.Id;
  const admin1ToProvinceId = {};
  for (const [key, info] of Object.entries(admin1Lookup)) {
    if (geoToProvinceId[info.geonameId]) admin1ToProvinceId[key] = geoToProvinceId[info.geonameId];
  }

  const countriesWithAdmin2 = new Set();
  for (const d of admin2Districts) countriesWithAdmin2.add(d.countryCode);

  // allCountries'den koordinat + population + timezone ekle
  for (const d of admin2Districts) {
    const coords = geoCoords.get(d.geonameId);
    if (coords) {
      d.latitude = coords.lat;
      d.longitude = coords.lng;
      d.population = coords.population;
      d.timezone = coords.timezone;
    }
  }

  // Birlesik: admin2 + cities fallback
  const allDistricts = [...admin2Districts];
  for (const c of citiesFallback) {
    if (countriesWithAdmin2.has(c.countryCode)) continue;
    // Cities fallback icin de allCountries'den zenginlestirilmis veri kullan
    const coords = geoCoords.get(c.geonameId);
    if (coords) {
      c.population = coords.population || c.population;
      c.timezone = coords.timezone || c.timezone;
    }
    allDistricts.push({ ...c, source: "cities" });
  }

  // Siralama: province → nufus DESC (buyuk ilceler uste)
  allDistricts.sort((a, b) => {
    const pa = admin1ToProvinceId[a.admin1Key] || 0;
    const pb = admin1ToProvinceId[b.admin1Key] || 0;
    if (pa !== pb) return pa - pb;
    return (b.population || 0) - (a.population || 0);
  });

  const result = [];
  const provinceSeq = {};
  for (const d of allDistricts) {
    const provinceId = admin1ToProvinceId[d.admin1Key];
    if (!provinceId) continue;
    provinceSeq[provinceId] = (provinceSeq[provinceId] || 0) + 1;
    const seq = provinceSeq[provinceId];
    const districtId = provinceId * 1000 + seq;
    if (districtId > 2_000_000_000) { console.log(`  UYARI: District ID tasmasi! provinceId=${provinceId}`); continue; }
    const nameTrans = translations[d.geonameId] || null;
    result.push({
      Id: districtId, ProvinceId: provinceId, Name: d.name,
      NameTranslations: nameTrans ? JSON.stringify(nameTrans) : null,
      GeoNameId: d.geonameId,
      Latitude: d.latitude, Longitude: d.longitude,
      Population: d.population || null,
      Timezone: d.timezone || null,
      SortOrder: seq,
    });
  }
  return result;
}

function buildNeighborhoodsJson(rawNeighborhoods, districtsJson) {
  // admin2GeonameId → District.Id mapping
  const admin2GeoToDistrictId = {};
  for (const dj of districtsJson) admin2GeoToDistrictId[dj.GeoNameId] = dj.Id;

  const result = [];
  const districtSeq = {};
  let globalSeq = 0;
  let skipped = 0;

  for (const n of rawNeighborhoods) {
    const districtId = admin2GeoToDistrictId[n.admin2GeonameId];
    if (!districtId) { skipped++; continue; }
    districtSeq[districtId] = (districtSeq[districtId] || 0) + 1;
    const seq = districtSeq[districtId];
    globalSeq++;

    result.push({
      Id: globalSeq,
      DistrictId: districtId,
      Name: n.name,
      Latitude: n.lat,
      Longitude: n.lng,
      PostalCode: null,
      GeoNameId: n.geonameId,
      SortOrder: seq,
    });
  }
  if (skipped > 0) console.log(`  ${skipped.toLocaleString()} neighborhood eslestirilemedi (admin2 yok).`);
  return result;
}

// ─── Main ──────────────────────────────────────────────

async function main() {
  console.log("=".repeat(60));
  console.log("GeoNames Location Data Generator (allCountries enriched)");
  console.log("=".repeat(60));

  // 1. Country mapping
  console.log("\n[1/8] Country mapping yukleniyor...");
  const countryMap = loadCountryMapping();
  console.log(`  ${Object.keys(countryMap).length} ulke.`);

  // 2. Admin1
  console.log("\n[2/8] Admin1 (il/eyalet) verileri...");
  const admin1Data = await download(ADMIN1_URL, "admin1CodesASCII.txt");
  const rawProvinces = parseAdmin1(admin1Data, countryMap);
  console.log(`  ${rawProvinces.length} il/eyalet.`);
  const admin1Lookup = {};
  for (const p of rawProvinces) admin1Lookup[p.codeFull] = p;

  // 3. Admin2
  console.log("\n[3/8] Admin2 (ilce/county) verileri...");
  const admin2Data = await download(ADMIN2_URL, "admin2Codes.txt");
  const admin2Districts = parseAdmin2(admin2Data, countryMap, admin1Lookup);
  const countriesWithAdmin2 = new Set(admin2Districts.map(d => d.countryCode));
  console.log(`  ${admin2Districts.length} ilce (${countriesWithAdmin2.size} ulke).`);

  // admin2Key → geonameId mapping (neighborhoods icin)
  const admin2KeyToGeonameId = new Map();
  for (const d of admin2Districts) {
    const admin1Code = admin1Lookup[d.admin1Key]?.adminCode;
    if (admin1Code) {
      const a2key = `${d.countryCode}.${admin1Code}.${d.admin2Code}`;
      admin2KeyToGeonameId.set(a2key, d.geonameId);
    }
  }

  // 4. Cities5000 (sadece fallback ulkeler icin)
  console.log("\n[4/8] Cities5000 (fallback) verileri...");
  const citiesData = await download(CITIES_URL, "cities5000.zip");
  const cities = parseCities(citiesData, countryMap, admin1Lookup);
  const citiesFallback = cities.filter(c => !countriesWithAdmin2.has(c.countryCode));
  console.log(`  ${cities.length} sehir, ${citiesFallback.length} fallback.`);

  // 5. allCountries (TEK GECIS: koordinat + population + timezone + neighborhoods)
  console.log("\n[5/8] allCountries.txt (koordinat + mahalle)...");
  const allCountriesZipPath = path.join(SCRIPT_DIR, "_tmp_allcountries.zip");
  if (!fs.existsSync(allCountriesZipPath)) {
    await downloadToFile(ALLCOUNTRIES_URL, allCountriesZipPath, "allCountries.zip");
  } else {
    console.log(`  ${allCountriesZipPath} zaten mevcut.`);
  }

  // Koordinat istenen tum geonameId'ler
  const targetGeonameIds = new Set();
  for (const p of rawProvinces) targetGeonameIds.add(p.geonameId);
  for (const d of admin2Districts) targetGeonameIds.add(d.geonameId);
  for (const c of cities) targetGeonameIds.add(c.geonameId);
  console.log(`  ${targetGeonameIds.size} geonameId icin koordinat aranacak.`);

  const { geoCoords, rawNeighborhoods } = await parseAllCountries(
    allCountriesZipPath, targetGeonameIds, admin2KeyToGeonameId, countryMap, admin1Lookup
  );

  // 6. Translations (sadece province + district + cities icin, neighborhood haric — cok fazla kayit)
  console.log("\n[6/8] Alternate names (ceviriler)...");
  console.log(`  ${targetGeonameIds.size} benzersiz GeoNameId (province + district + cities).`);

  const altNamesZipPath = path.join(SCRIPT_DIR, "_tmp_altnames.zip");
  if (!fs.existsSync(altNamesZipPath)) {
    await downloadToFile(ALT_NAMES_URL, altNamesZipPath, "alternateNamesV2.zip");
  } else {
    console.log(`  ${altNamesZipPath} zaten mevcut.`);
  }
  const translations = await parseAlternateNames(altNamesZipPath, targetGeonameIds);

  // 7. Build JSON
  console.log("\n[7/8] JSON dosyalari olusturuluyor...");
  const provincesJson = buildProvincesJson(rawProvinces, translations, geoCoords);
  const districtsJson = buildDistrictsJson(admin2Districts, citiesFallback, provincesJson, admin1Lookup, translations, geoCoords);
  const neighborhoodsJson = buildNeighborhoodsJson(rawNeighborhoods, districtsJson);

  // 8. Write files
  console.log("\n[8/8] Dosyalar yaziliyor...");
  const write = (name, data, compact = false) => {
    const filePath = path.join(SCRIPT_DIR, name);
    if (compact) {
      // Buyuk dosyalar icin satirlik JSON (hafiza verimli)
      const ws = fs.createWriteStream(filePath, { encoding: "utf8" });
      ws.write("[\n");
      for (let i = 0; i < data.length; i++) {
        ws.write(JSON.stringify(data[i]));
        if (i < data.length - 1) ws.write(",\n");
      }
      ws.write("\n]");
      ws.end();
    } else {
      fs.writeFileSync(filePath, JSON.stringify(data, null, 2), "utf8");
    }
    // writeStream async oldugundan boyutu hemen alamayabiliriz, skip et
    if (!compact) {
      const size = Math.floor(fs.statSync(filePath).size / 1024);
      console.log(`  ${name}: ${data.length.toLocaleString()} kayit (${size.toLocaleString()}KB)`);
    } else {
      console.log(`  ${name}: ${data.length.toLocaleString()} kayit (compact)`);
    }
  };
  write("provinces.json", provincesJson);
  write("districts.json", districtsJson);
  write("neighborhoods.json", neighborhoodsJson, true);

  // Summary
  const withCoord = districtsJson.filter(d => d.Latitude !== null).length;
  const withPop = districtsJson.filter(d => d.Population > 0).length;
  const withTz = districtsJson.filter(d => d.Timezone).length;
  const provWithCoord = provincesJson.filter(p => p.Latitude !== null).length;
  console.log(`\n${"=".repeat(60)}`);
  console.log(`OZET:`);
  console.log(`  Il/eyalet: ${provincesJson.length.toLocaleString()} (${provWithCoord.toLocaleString()} koordinatli)`);
  console.log(`  Ilce/sehir: ${districtsJson.length.toLocaleString()}`);
  console.log(`    - Koordinatli: ${withCoord.toLocaleString()}`);
  console.log(`    - Population: ${withPop.toLocaleString()}`);
  console.log(`    - Timezone: ${withTz.toLocaleString()}`);
  console.log(`  Mahalle/koyu: ${neighborhoodsJson.length.toLocaleString()}`);
  console.log(`  Admin2 ulke: ${countriesWithAdmin2.size}, Cities fallback: ${new Set(provincesJson.map(p => p.CountryId)).size - countriesWithAdmin2.size}`);
  console.log(`${"=".repeat(60)}`);
}

main().catch((err) => {
  console.error("HATA:", err.message, err.stack);
  process.exit(1);
});
