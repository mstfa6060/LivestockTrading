#!/usr/bin/env node
/**
 * GeoNames veri indirme ve provinces.json / districts.json olusturma scripti.
 *
 * Kaynak: https://download.geonames.org/export/dump/
 * - admin1CodesASCII.txt: Il/Eyalet/Bolge (admin1 divisions)
 * - admin2Codes.txt: Ilce/County (admin2 divisions) — resmi idari bolumler
 * - cities15000.zip: Nufusu 15000+ sehirler (admin2 olmayan ulkeler icin fallback)
 * - alternateNamesV2.zip: Coklu dil cevirileri
 *
 * Hibrit strateji:
 *   1. admin2Codes olan ulkelerde → admin2 kayitlari kullanilir (resmi ilceler)
 *   2. admin2 olmayan ulkelerde → cities15000 fallback
 *   3. Koordinatlar: cities5000'den admin1+admin2 code eslemesi ile bulunur
 *
 * Kullanim: node generate_locations.js
 */

const https = require("https");
const http = require("http");
const fs = require("fs");
const path = require("path");
const AdmZip = require("adm-zip");

const SCRIPT_DIR = __dirname;

const ADMIN1_URL = "https://download.geonames.org/export/dump/admin1CodesASCII.txt";
const ADMIN2_URL = "https://download.geonames.org/export/dump/admin2Codes.txt";
const CITIES_URL = "https://download.geonames.org/export/dump/cities5000.zip";
const ALT_NAMES_URL = "https://download.geonames.org/export/dump/alternateNamesV2.zip";

const TARGET_LANGUAGES = new Set([
  "en","tr","de","fr","es","ar","ru","zh","ja","ko","pt","it","nl","pl","hi",
  "fa","ur","az","id","th","vi","uk","ro","el","he","sv","da","no","fi","cs",
  "hu","bg","hr","sr","sk","sl","lt","lv","et","ka","hy","ms","tl","sw","bn",
  "ta","te","mr","gu","pa"
]);

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
      res.on("end", () => {
        process.stdout.write("\n");
        resolve(Buffer.concat(chunks));
      });
      res.on("error", reject);
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
      res.on("end", () => {
        ws.end();
        process.stdout.write("\n");
        resolve();
      });
      res.on("error", (err) => { ws.end(); reject(err); });
    }).on("error", reject);
  });
}

// ─── Parsers ───────────────────────────────────────────

function loadCountryMapping() {
  const filePath = path.join(SCRIPT_DIR, "countries.json");
  const countries = JSON.parse(fs.readFileSync(filePath, "utf8"));
  const map = {};
  for (const c of countries) {
    map[c.Code] = c.Id;
  }
  return map;
}

function parseAdmin1(content, countryMap) {
  const lines = content.toString("utf8").split("\n");
  const provinces = [];
  for (const line of lines) {
    const parts = line.trim().split("\t");
    if (parts.length < 4) continue;
    const codeFull = parts[0]; // e.g. "TR.34"
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

/**
 * admin2Codes.txt parse et.
 * Format: CC.ADMIN1.ADMIN2\tName\tASCII Name\tGeoNameId
 * Ornek: TR.34.7732579\tPendik\tPendik\t7732579
 */
function parseAdmin2(content, countryMap, admin1Lookup) {
  const lines = content.toString("utf8").split("\n");
  const districts = [];
  for (const line of lines) {
    const parts = line.trim().split("\t");
    if (parts.length < 4) continue;
    const codeFull = parts[0]; // e.g. "TR.34.7732579"
    const name = parts[1];
    const geonameId = parseInt(parts[3], 10);

    const firstDot = codeFull.indexOf(".");
    if (firstDot < 0) continue;
    const cc = codeFull.substring(0, firstDot);
    const rest = codeFull.substring(firstDot + 1); // "34.7732579"
    const secondDot = rest.indexOf(".");
    if (secondDot < 0) continue;
    const admin1Code = rest.substring(0, secondDot);

    const countryId = countryMap[cc];
    if (!countryId) continue;

    const admin1Key = `${cc}.${admin1Code}`;
    if (!admin1Lookup[admin1Key]) continue;

    districts.push({
      geonameId,
      name,
      countryCode: cc,
      admin1Key,
      admin2Code: rest.substring(secondDot + 1),
      population: 0, // admin2 has no population data
      latitude: null,
      longitude: null,
      source: "admin2",
    });
  }
  return districts;
}

/**
 * cities5000 parse et — admin2 koordinatlarini bulmak + admin2 olmayan ulkeler icin fallback.
 * Ayrica admin2 geonameId → {lat, lng} lookup tablosu olusturur.
 */
function parseCities(zipBuffer, countryMap, admin1Lookup) {
  const cities = [];
  const geoIdCoords = {}; // geonameId → { lat, lng }
  // admin2Key (CC.admin1.admin2) → best city coords (en buyuk nufuslu)
  const admin2Coords = {};

  // admin1Key|lowercase_name → { lat, lng, population } (isim eslesmesi icin)
  const cityNameIndex = {};
  // admin1Key → { lat, lng } (admin1'in en buyuk sehrinin koordinati — fallback icin)
  const admin1CenterCoords = {};

  const entries = extractZipEntry(zipBuffer, "cities5000");
  for (const entry of entries) {
    const lines = entry.data.toString("utf8").split("\n");
    for (const line of lines) {
      const parts = line.split("\t");
      if (parts.length < 15) continue;
      const geonameId = parseInt(parts[0], 10);
      const cityName = parts[1];
      const latitude = parseFloat(parts[4]) || null;
      const longitude = parseFloat(parts[5]) || null;
      const cc = parts[8];
      const admin1Code = parts[10];
      const admin2Code = parts[11];
      const population = parseInt(parts[14], 10) || 0;
      const countryId = countryMap[cc];
      if (!countryId) continue;
      const admin1Key = `${cc}.${admin1Code}`;
      if (!admin1Lookup[admin1Key]) continue;

      // Koordinat lookup
      geoIdCoords[geonameId] = { lat: latitude, lng: longitude };

      // admin2 koordinat lookup (en buyuk nufuslu sehir)
      if (admin2Code) {
        const a2key = `${cc}.${admin1Code}.${admin2Code}`;
        if (!admin2Coords[a2key] || population > admin2Coords[a2key].population) {
          admin2Coords[a2key] = { lat: latitude, lng: longitude, population };
        }
      }

      // Isim tabanlı lookup (en buyuk nufuslu olan tercih edilir)
      const nameKey = `${admin1Key}|${cityName.toLowerCase()}`;
      if (!cityNameIndex[nameKey] || population > cityNameIndex[nameKey].population) {
        cityNameIndex[nameKey] = { lat: latitude, lng: longitude, population };
      }

      // Admin1 merkez koordinati (en buyuk nufuslu sehir)
      if (!admin1CenterCoords[admin1Key] || population > admin1CenterCoords[admin1Key].population) {
        admin1CenterCoords[admin1Key] = { lat: latitude, lng: longitude, population };
      }

      cities.push({ geonameId, name: cityName, countryCode: cc, admin1Key, admin2Code, population, latitude, longitude });
    }
  }
  return { cities, geoIdCoords, admin2Coords, cityNameIndex, admin1CenterCoords };
}

function parseAlternateNames(zipBuffer, targetGeonameIds) {
  console.log("  alternateNamesV2 parse ediliyor (buyuk dosya, satir satir)...");
  const translations = {}; // geonameId → { lang: name }

  const tmpDir = path.join(SCRIPT_DIR, "_tmp_geonames");
  if (!fs.existsSync(tmpDir)) fs.mkdirSync(tmpDir);

  const zip = new AdmZip(zipBuffer);
  const entry = zip.getEntries().find(e => e.entryName.includes("alternateNamesV2") && !e.isDirectory);
  if (!entry) {
    console.log("  WARN: alternateNamesV2.txt entry bulunamadi ZIP icinde.");
    return translations;
  }

  const tmpFile = path.join(tmpDir, "alternateNamesV2.txt");
  console.log("  ZIP'ten diske cikariliyor...");
  zip.extractEntryTo(entry, tmpDir, false, true);

  const readline = require("readline");
  const rl = readline.createInterface({
    input: fs.createReadStream(tmpFile, { encoding: "utf8" }),
    crlfDelay: Infinity,
  });

  let lineCount = 0;
  return new Promise((resolve) => {
    rl.on("line", (line) => {
      lineCount++;
      if (lineCount % 5_000_000 === 0) {
        process.stdout.write(`\r  ${Math.floor(lineCount / 1_000_000)}M satir islendi...`);
      }
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
      if (!translations[gid][lang] || isPreferred) {
        translations[gid][lang] = altName;
      }
    });

    rl.on("close", () => {
      const count = Object.keys(translations).length;
      console.log(`\r  ${Math.floor(lineCount / 1_000_000)}M satir islendi. ${count} kayit icin ceviri bulundu.`);
      try { fs.unlinkSync(tmpFile); fs.rmdirSync(tmpDir); } catch {}
      resolve(translations);
    });
  });
}

// ─── Builders ──────────────────────────────────────────

function buildProvincesJson(rawProvinces, translations) {
  rawProvinces.sort((a, b) => a.countryId - b.countryId || a.name.localeCompare(b.name));
  const result = [];
  const countrySeq = {};

  for (const p of rawProvinces) {
    const cid = p.countryId;
    countrySeq[cid] = (countrySeq[cid] || 0) + 1;
    const seq = countrySeq[cid];
    const provinceId = cid * 10000 + seq;

    const nameTrans = translations[p.geonameId] || null;
    const nameTransJson = nameTrans ? JSON.stringify(nameTrans) : null;

    result.push({
      Id: provinceId,
      CountryId: cid,
      Name: p.name,
      Code: p.adminCode,
      NameTranslations: nameTransJson,
      GeoNameId: p.geonameId,
      SortOrder: seq,
    });
  }
  return result;
}

/**
 * Hibrit district builder:
 * - admin2 olan ulkelerde: admin2 kayitlari kullanilir
 * - admin2 olmayan ulkelerde: cities5000 fallback
 */
function buildDistrictsJson(admin2Districts, citiesFallback, provincesJson, admin1Lookup, translations, geoIdCoords, admin2Coords, cityNameIndex, admin1CenterCoords) {
  // Province GeoNameId → Province.Id
  const geoToProvinceId = {};
  for (const pj of provincesJson) {
    geoToProvinceId[pj.GeoNameId] = pj.Id;
  }

  // admin1Key → Province.Id
  const admin1ToProvinceId = {};
  // admin1Key → Province.Code (admin1Code)
  const admin1ToCode = {};
  for (const [key, info] of Object.entries(admin1Lookup)) {
    if (geoToProvinceId[info.geonameId]) {
      admin1ToProvinceId[key] = geoToProvinceId[info.geonameId];
      admin1ToCode[key] = info.adminCode;
    }
  }

  // Hangi ulkelerin admin2 verisi var?
  const countriesWithAdmin2 = new Set();
  for (const d of admin2Districts) {
    countriesWithAdmin2.add(d.countryCode);
  }

  // admin2 district'lere koordinat ekle
  for (const d of admin2Districts) {
    // Oncelik 1: geoIdCoords'dan direkt geonameId ile bul
    if (geoIdCoords[d.geonameId]) {
      d.latitude = geoIdCoords[d.geonameId].lat;
      d.longitude = geoIdCoords[d.geonameId].lng;
      continue;
    }
    // Oncelik 2: admin2Coords'dan admin1+admin2 code ile bul
    const admin1Code = admin1ToCode[d.admin1Key];
    if (admin1Code && d.admin2Code) {
      const a2key = `${d.countryCode}.${admin1Code}.${d.admin2Code}`;
      if (admin2Coords[a2key]) {
        d.latitude = admin2Coords[a2key].lat;
        d.longitude = admin2Coords[a2key].lng;
        continue;
      }
    }
    // Oncelik 3: ayni admin1 altinda isim eslesmesi ile bul
    const nameKey = `${d.admin1Key}|${d.name.toLowerCase()}`;
    if (cityNameIndex[nameKey]) {
      d.latitude = cityNameIndex[nameKey].lat;
      d.longitude = cityNameIndex[nameKey].lng;
      continue;
    }
    // Oncelik 4: admin1'in en buyuk sehrinin koordinatini fallback olarak kullan
    if (admin1CenterCoords[d.admin1Key]) {
      d.latitude = admin1CenterCoords[d.admin1Key].lat;
      d.longitude = admin1CenterCoords[d.admin1Key].lng;
    }
  }

  // Birlesik district listesi: admin2 ulkeleri icin admin2, digerleri icin cities fallback
  const allDistricts = [];

  for (const d of admin2Districts) {
    allDistricts.push(d);
  }

  // Cities fallback: sadece admin2 olmayan ulkelerdeki sehirler
  for (const c of citiesFallback) {
    if (countriesWithAdmin2.has(c.countryCode)) continue;
    allDistricts.push({
      ...c,
      source: "cities",
    });
  }

  // Sort: admin2 kayitlari ada gore, cities nufusa gore
  allDistricts.sort((a, b) => {
    const pa = admin1ToProvinceId[a.admin1Key] || 0;
    const pb = admin1ToProvinceId[b.admin1Key] || 0;
    if (pa !== pb) return pa - pb;
    // Admin2: ada gore, Cities: nufusa gore
    if (a.source === "admin2" && b.source === "admin2") return a.name.localeCompare(b.name);
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

    if (districtId > 2_000_000_000) {
      console.log(`  UYARI: District ID tasmasi! provinceId=${provinceId}, seq=${seq}`);
      continue;
    }

    const nameTrans = translations[d.geonameId] || null;
    const nameTransJson = nameTrans ? JSON.stringify(nameTrans) : null;

    result.push({
      Id: districtId,
      ProvinceId: provinceId,
      Name: d.name,
      NameTranslations: nameTransJson,
      GeoNameId: d.geonameId,
      Latitude: d.latitude,
      Longitude: d.longitude,
      SortOrder: seq,
    });
  }
  return result;
}

// ─── Main ──────────────────────────────────────────────

async function main() {
  console.log("=".repeat(60));
  console.log("GeoNames Location Data Generator (Hybrid: admin2 + cities)");
  console.log("=".repeat(60));

  // 1. Country mapping
  console.log("\n[1/7] Country mapping yukleniyor...");
  const countryMap = loadCountryMapping();
  console.log(`  ${Object.keys(countryMap).length} ulke eslesmesi yuklendi.`);

  // 2. Admin1
  console.log("\n[2/7] Admin1 (il/eyalet) verileri indiriliyor...");
  const admin1Data = await download(ADMIN1_URL, "admin1CodesASCII.txt");
  const rawProvinces = parseAdmin1(admin1Data, countryMap);
  console.log(`  ${rawProvinces.length} il/eyalet parse edildi.`);
  const admin1Lookup = {};
  for (const p of rawProvinces) admin1Lookup[p.codeFull] = p;

  // 3. Admin2 (resmi ilceler)
  console.log("\n[3/7] Admin2 (ilce/county) verileri indiriliyor...");
  const admin2Data = await download(ADMIN2_URL, "admin2Codes.txt");
  const admin2Districts = parseAdmin2(admin2Data, countryMap, admin1Lookup);
  const countriesWithAdmin2 = new Set(admin2Districts.map(d => d.countryCode));
  console.log(`  ${admin2Districts.length} ilce/county parse edildi (${countriesWithAdmin2.size} ulke).`);

  // 4. Cities5000 (koordinat kaynagi + fallback)
  console.log("\n[4/7] Cities5000 (sehir) verileri indiriliyor...");
  const citiesData = await download(CITIES_URL, "cities5000.zip");
  const { cities, geoIdCoords, admin2Coords, cityNameIndex, admin1CenterCoords } = parseCities(citiesData, countryMap, admin1Lookup);
  const citiesFallbackCount = cities.filter(c => !countriesWithAdmin2.has(c.countryCode)).length;
  console.log(`  ${cities.length} sehir parse edildi (${Object.keys(geoIdCoords).length} koordinat, ${citiesFallbackCount} fallback).`);

  // 5. Translations
  console.log("\n[5/7] Alternate names (ceviriler) indiriliyor...");
  const allGeonameIds = new Set();
  for (const p of rawProvinces) allGeonameIds.add(p.geonameId);
  for (const d of admin2Districts) allGeonameIds.add(d.geonameId);
  for (const c of cities) allGeonameIds.add(c.geonameId);
  console.log(`  ${allGeonameIds.size} benzersiz GeoNameId icin ceviri aranacak.`);

  const altNamesZipPath = path.join(SCRIPT_DIR, "_tmp_altnames.zip");
  if (fs.existsSync(altNamesZipPath)) {
    console.log(`  ${altNamesZipPath} zaten mevcut, indirme atlaniyor.`);
  } else {
    await downloadToFile(ALT_NAMES_URL, altNamesZipPath, "alternateNamesV2.zip");
  }
  const altNamesData = fs.readFileSync(altNamesZipPath);
  const translations = await parseAlternateNames(altNamesData, allGeonameIds);
  // Dosyayi silme — tekrar kullanilabilir
  // try { fs.unlinkSync(altNamesZipPath); } catch {}

  // 6. Build JSON
  console.log("\n[6/7] JSON dosyalari olusturuluyor...");
  const provincesJson = buildProvincesJson(rawProvinces, translations);
  const districtsJson = buildDistrictsJson(admin2Districts, cities, provincesJson, admin1Lookup, translations, geoIdCoords, admin2Coords, cityNameIndex, admin1CenterCoords);

  // 7. Write files
  console.log("\n[7/7] Dosyalar yaziliyor...");
  const provincesPath = path.join(SCRIPT_DIR, "provinces.json");
  const districtsPath = path.join(SCRIPT_DIR, "districts.json");

  fs.writeFileSync(provincesPath, JSON.stringify(provincesJson, null, 2), "utf8");
  const pSize = Math.floor(fs.statSync(provincesPath).size / 1024);
  console.log(`  provinces.json: ${provincesJson.length} kayit (${pSize}KB)`);

  fs.writeFileSync(districtsPath, JSON.stringify(districtsJson, null, 2), "utf8");
  const dSize = Math.floor(fs.statSync(districtsPath).size / 1024);
  console.log(`  districts.json: ${districtsJson.length} kayit (${dSize}KB)`);

  // Summary
  const countriesWithProvinces = new Set(provincesJson.map(p => p.CountryId)).size;
  const admin2Count = districtsJson.filter(d => d.Latitude !== null).length;
  const noCoordCount = districtsJson.filter(d => d.Latitude === null).length;
  console.log(`\n${"=".repeat(60)}`);
  console.log(`OZET:`);
  console.log(`  Ulke sayisi (il verisi olan): ${countriesWithProvinces}`);
  console.log(`  Toplam il/eyalet: ${provincesJson.length}`);
  console.log(`  Toplam ilce/sehir: ${districtsJson.length}`);
  console.log(`    - Koordinatli: ${admin2Count}`);
  console.log(`    - Koordinatsiz: ${noCoordCount}`);
  console.log(`  Admin2 kullanan ulke: ${countriesWithAdmin2.size}`);
  console.log(`  Cities fallback ulke: ${countriesWithProvinces - countriesWithAdmin2.size}`);
  console.log(`${"=".repeat(60)}`);
}

main().catch((err) => {
  console.error("HATA:", err.message);
  process.exit(1);
});
