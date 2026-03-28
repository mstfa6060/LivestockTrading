#!/usr/bin/env node
/**
 * GeoNames veri indirme ve provinces.json / districts.json olusturma scripti.
 *
 * Kaynak: https://download.geonames.org/export/dump/
 * - admin1CodesASCII.txt: Il/Eyalet/Bolge (admin1 divisions)
 * - cities15000.zip: Nufusu 15000+ sehirler
 * - alternateNamesV2.zip: Coklu dil cevirileri
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
const CITIES_URL = "https://download.geonames.org/export/dump/cities15000.zip";
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

function parseCities(zipBuffer, countryMap, admin1Lookup) {
  const districts = [];
  const entries = extractZipEntry(zipBuffer, "cities15000");
  for (const entry of entries) {
    const lines = entry.data.toString("utf8").split("\n");
    for (const line of lines) {
      const parts = line.split("\t");
      if (parts.length < 15) continue;
      const geonameId = parseInt(parts[0], 10);
      const cityName = parts[1];
      const cc = parts[8];
      const admin1Code = parts[10];
      const population = parseInt(parts[14], 10) || 0;
      const countryId = countryMap[cc];
      if (!countryId) continue;
      const admin1Key = `${cc}.${admin1Code}`;
      if (!admin1Lookup[admin1Key]) continue;
      districts.push({ geonameId, name: cityName, countryCode: cc, admin1Key, population });
    }
  }
  return districts;
}

function parseAlternateNames(zipBuffer, targetGeonameIds) {
  console.log("  alternateNamesV2 parse ediliyor (buyuk dosya, satir satir)...");
  const translations = {}; // geonameId → { lang: name }

  // ZIP'ten dosyayi diske cikar, sonra satir satir oku (memory-safe)
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

  // Satir satir oku (stream)
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

      // Temizlik
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

function buildDistrictsJson(rawDistricts, provincesJson, admin1Lookup, translations) {
  // GeoNameId → Province.Id mapping
  const geoToProvinceId = {};
  for (const pj of provincesJson) {
    geoToProvinceId[pj.GeoNameId] = pj.Id;
  }

  // admin1Key → Province.Id
  const admin1ToProvinceId = {};
  for (const [key, info] of Object.entries(admin1Lookup)) {
    if (geoToProvinceId[info.geonameId]) {
      admin1ToProvinceId[key] = geoToProvinceId[info.geonameId];
    }
  }

  // Sort by province then population desc
  rawDistricts.sort((a, b) => {
    const pa = admin1ToProvinceId[a.admin1Key] || 0;
    const pb = admin1ToProvinceId[b.admin1Key] || 0;
    return pa - pb || b.population - a.population;
  });

  const result = [];
  const provinceSeq = {};

  for (const d of rawDistricts) {
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
      SortOrder: seq,
    });
  }
  return result;
}

// ─── Main ──────────────────────────────────────────────

async function main() {
  console.log("=".repeat(60));
  console.log("GeoNames Location Data Generator (Node.js)");
  console.log("=".repeat(60));

  // 1. Country mapping
  console.log("\n[1/6] Country mapping yukleniyor...");
  const countryMap = loadCountryMapping();
  console.log(`  ${Object.keys(countryMap).length} ulke eslesmesi yuklendi.`);

  // 2. Admin1
  console.log("\n[2/6] Admin1 (il/eyalet) verileri indiriliyor...");
  const admin1Data = await download(ADMIN1_URL, "admin1CodesASCII.txt");
  const rawProvinces = parseAdmin1(admin1Data, countryMap);
  console.log(`  ${rawProvinces.length} il/eyalet parse edildi.`);
  const admin1Lookup = {};
  for (const p of rawProvinces) admin1Lookup[p.codeFull] = p;

  // 3. Cities
  console.log("\n[3/6] Cities15000 (sehir) verileri indiriliyor...");
  const citiesData = await download(CITIES_URL, "cities15000.zip");
  const rawDistricts = parseCities(citiesData, countryMap, admin1Lookup);
  console.log(`  ${rawDistricts.length} sehir parse edildi.`);

  // 4. Translations
  console.log("\n[4/6] Alternate names (ceviriler) indiriliyor...");
  const allGeonameIds = new Set();
  for (const p of rawProvinces) allGeonameIds.add(p.geonameId);
  for (const d of rawDistricts) allGeonameIds.add(d.geonameId);
  console.log(`  ${allGeonameIds.size} benzersiz GeoNameId icin ceviri aranacak.`);

  // Buyuk dosyayi diske indir (RAM'de tutma)
  const altNamesZipPath = path.join(SCRIPT_DIR, "_tmp_altnames.zip");
  await downloadToFile(ALT_NAMES_URL, altNamesZipPath, "alternateNamesV2.zip");
  const altNamesData = fs.readFileSync(altNamesZipPath);
  const translations = await parseAlternateNames(altNamesData, allGeonameIds);
  try { fs.unlinkSync(altNamesZipPath); } catch {}

  // 5. Build JSON
  console.log("\n[5/6] JSON dosyalari olusturuluyor...");
  const provincesJson = buildProvincesJson(rawProvinces, translations);
  const districtsJson = buildDistrictsJson(rawDistricts, provincesJson, admin1Lookup, translations);

  // 6. Write files
  console.log("\n[6/6] Dosyalar yaziliyor...");
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
  const provWithTrans = provincesJson.filter(p => p.NameTranslations).length;
  const distWithTrans = districtsJson.filter(d => d.NameTranslations).length;
  console.log(`\n${"=".repeat(60)}`);
  console.log(`OZET:`);
  console.log(`  Ulke sayisi (il verisi olan): ${countriesWithProvinces}`);
  console.log(`  Toplam il/eyalet: ${provincesJson.length}`);
  console.log(`  Toplam sehir/ilce: ${districtsJson.length}`);
  console.log(`  Ceviri: ${provWithTrans} province + ${distWithTrans} district`);
  console.log(`${"=".repeat(60)}`);
}

main().catch((err) => {
  console.error("HATA:", err.message);
  process.exit(1);
});
