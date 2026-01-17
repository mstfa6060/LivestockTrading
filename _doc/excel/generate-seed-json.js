const XLSX = require('xlsx');
const fs = require('fs');
const path = require('path');

// Title case fonksiyonu - Türkçe karakterlere uygun
function toTitleCase(str) {
  if (!str) return '';

  return str.split(' ').map(word => {
    if (word.length === 0) return word;

    // Tüm harfleri küçült (Türkçe uyumlu)
    let lower = '';
    for (let i = 0; i < word.length; i++) {
      const char = word[i];
      if (char === 'I') lower += 'ı';
      else if (char === 'İ') lower += 'i';
      else lower += char.toLowerCase();
    }

    // İlk harfi büyüt (Türkçe uyumlu)
    const firstChar = lower[0];
    let firstUpper;
    if (firstChar === 'i') firstUpper = 'İ';
    else if (firstChar === 'ı') firstUpper = 'I';
    else firstUpper = firstChar.toUpperCase();

    return firstUpper + lower.slice(1);
  }).join(' ');
}

console.log('Reading Excel file...');
const wb = XLSX.readFile('Il_Ilce_MahalleKoy_Birlesik.xlsx');
const sheet = wb.Sheets['Tüm Veriler'];
const data = XLSX.utils.sheet_to_json(sheet, { header: 1 });
const rows = data.slice(1); // Skip header

console.log(`Total rows: ${rows.length}`);

// Excel'deki tüm unique il isimlerini al
const excelProvinces = [...new Set(rows.map(r => r[0]))].filter(Boolean);
console.log(`Unique provinces in Excel: ${excelProvinces.length}`);

// Plaka kodları sırasına göre il isimleri - Excel'deki yazımla eşleşecek şekilde
// Elle kontrol ettim - Excel'de İ harfi 0x130 (Latin Capital Letter I with Dot Above) olarak geçiyor
const plateCodeOrder = [
  'ADANA', 'ADIYAMAN', 'AFYONKARAHİSAR', 'AĞRI', 'AMASYA', 'ANKARA', 'ANTALYA', 'ARTVİN',
  'AYDIN', 'BALIKESİR', 'BİLECİK', 'BİNGÖL', 'BİTLİS', 'BOLU', 'BURDUR', 'BURSA',
  'ÇANAKKALE', 'ÇANKIRI', 'ÇORUM', 'DENİZLİ', 'DİYARBAKIR', 'EDİRNE', 'ELAZIĞ', 'ERZİNCAN',
  'ERZURUM', 'ESKİŞEHİR', 'GAZİANTEP', 'GİRESUN', 'GÜMÜŞHANE', 'HAKKARİ', 'HATAY', 'ISPARTA',
  'MERSİN', 'İSTANBUL', 'İZMİR', 'KARS', 'KASTAMONU', 'KAYSERİ', 'KIRKLARELİ', 'KIRŞEHİR',
  'KOCAELİ', 'KONYA', 'KÜTAHYA', 'MALATYA', 'MANİSA', 'KAHRAMANMARAŞ', 'MARDİN', 'MUĞLA',
  'MUŞ', 'NEVŞEHİR', 'NİĞDE', 'ORDU', 'RİZE', 'SAKARYA', 'SAMSUN', 'SİİRT',
  'SİNOP', 'SİVAS', 'TEKİRDAĞ', 'TOKAT', 'TRABZON', 'TUNCELİ', 'ŞANLIURFA', 'UŞAK',
  'VAN', 'YOZGAT', 'ZONGULDAK', 'AKSARAY', 'BAYBURT', 'KARAMAN', 'KIRIKKALE', 'BATMAN',
  'ŞIRNAK', 'BARTIN', 'ARDAHAN', 'IĞDIR', 'YALOVA', 'KARABÜK', 'KİLİS', 'OSMANİYE', 'DÜZCE'
];

// Excel'den gelen il isimlerini plaka sırasına eşle
// Excel isimleri ile plaka listesi arasındaki farkları tespit et
const excelToPlate = {};
excelProvinces.forEach(excelName => {
  // Plaka listesinde ara
  const found = plateCodeOrder.find(plateName => {
    // Normalize edip karşılaştır
    return plateName.normalize('NFC') === excelName.normalize('NFC');
  });

  if (found) {
    excelToPlate[excelName] = plateCodeOrder.indexOf(found) + 1;
  } else {
    console.warn(`Excel province not matched: "${excelName}"`);
    // Hala eşleşmediyse, en yakın olanı bul (levenshtein kullanmadan basit karşılaştırma)
    for (let i = 0; i < plateCodeOrder.length; i++) {
      if (plateCodeOrder[i].replace(/İ/g, 'I') === excelName.replace(/İ/g, 'I')) {
        excelToPlate[excelName] = i + 1;
        console.log(`  -> Matched to: ${plateCodeOrder[i]} (plate ${i + 1})`);
        break;
      }
    }
  }
});

// 1. Provinces - Excel'deki isimlerden, plaka sırasına göre
const provinceMap = {}; // Excel name -> plate code (id)

// Her Excel il adını plaka koduyla eşle
excelProvinces.forEach(excelName => {
  if (!excelToPlate[excelName]) {
    // Eşleşme yoksa, Excel'deki sıraya göre bir ID ver
    console.warn(`No plate code for: ${excelName}`);
  }
});

// Provinces listesi oluştur - plaka sırasına göre
const provinces = [];
for (let plateCode = 1; plateCode <= 81; plateCode++) {
  // Bu plaka koduna sahip Excel ismini bul
  const excelName = Object.keys(excelToPlate).find(name => excelToPlate[name] === plateCode);
  if (excelName) {
    provinceMap[excelName] = plateCode;
    provinces.push({
      id: plateCode,
      name: toTitleCase(excelName),
      code: String(plateCode).padStart(2, '0'),
      sortOrder: plateCode
    });
  } else {
    console.warn(`No Excel match for plate code: ${plateCode}`);
  }
}

console.log(`Provinces created: ${provinces.length}`);

// 2. Districts
const districtSet = new Map();
rows.forEach(r => {
  const provinceName = r[0];
  const districtName = r[1];
  if (provinceName && districtName) {
    const key = provinceName + '|' + districtName;
    if (!districtSet.has(key)) {
      districtSet.set(key, { provinceName, districtName });
    }
  }
});

// District'ları il içinde alfabetik sırala
const districtArray = Array.from(districtSet.values());
districtArray.sort((a, b) => {
  const pA = provinceMap[a.provinceName] || 999;
  const pB = provinceMap[b.provinceName] || 999;
  if (pA !== pB) return pA - pB;
  return a.districtName.localeCompare(b.districtName, 'tr');
});

let districtId = 1;
const districtMap = {};
const districts = districtArray.map((d, idx) => {
  const provinceId = provinceMap[d.provinceName];
  if (!provinceId) {
    console.warn(`Province not found for district: ${d.provinceName} - ${d.districtName}`);
    return null;
  }
  const key = d.provinceName + '|' + d.districtName;
  const id = districtId++;
  districtMap[key] = id;
  return {
    id,
    name: toTitleCase(d.districtName),
    provinceId,
    sortOrder: idx + 1
  };
}).filter(Boolean);

console.log(`Districts created: ${districts.length}`);

// 3. Neighborhoods - ilçe içinde sıralı
const neighborhoodsByDistrict = new Map();
rows.forEach(r => {
  const provinceName = r[0];
  const districtName = r[1];
  const neighborhoodName = r[2];

  if (!provinceName || !districtName || !neighborhoodName) return;

  const key = provinceName + '|' + districtName;
  if (!neighborhoodsByDistrict.has(key)) {
    neighborhoodsByDistrict.set(key, []);
  }
  neighborhoodsByDistrict.get(key).push(neighborhoodName);
});

// Her ilçe için mahalleri alfabetik sırala
let neighborhoodId = 1;
const neighborhoods = [];

// İlçe sırasına göre işle
districts.forEach(district => {
  // Bu ilçeye ait key'i bul
  const matchingKey = Array.from(neighborhoodsByDistrict.keys()).find(key => {
    return districtMap[key] === district.id;
  });

  if (!matchingKey) return;

  const names = neighborhoodsByDistrict.get(matchingKey);
  // Alfabetik sırala
  names.sort((a, b) => a.localeCompare(b, 'tr'));

  names.forEach((name, idx) => {
    neighborhoods.push({
      id: neighborhoodId++,
      name: toTitleCase(name),
      districtId: district.id,
      postalCode: '',
      sortOrder: idx + 1
    });
  });
});

console.log(`Neighborhoods created: ${neighborhoods.length}`);

// SeedData klasörüne yaz - doğru yol
// __dirname = d:\Projects\Maden\backend\_doc\excel
// Hedef   = d:\Projects\Maden\backend\Jobs\RelationalDB\MigrationJob\SeedData
const seedPath = path.resolve(__dirname, '..', '..', 'Jobs', 'RelationalDB', 'MigrationJob', 'SeedData');

// Klasör yoksa oluştur
if (!fs.existsSync(seedPath)) {
  fs.mkdirSync(seedPath, { recursive: true });
}

console.log(`\nWriting to: ${seedPath}`);

fs.writeFileSync(path.join(seedPath, 'provinces.json'), JSON.stringify(provinces, null, 2), 'utf8');
console.log('✓ provinces.json written');

fs.writeFileSync(path.join(seedPath, 'districts.json'), JSON.stringify(districts, null, 2), 'utf8');
console.log('✓ districts.json written');

fs.writeFileSync(path.join(seedPath, 'neighborhoods.json'), JSON.stringify(neighborhoods, null, 2), 'utf8');
console.log('✓ neighborhoods.json written');

console.log('\n All JSON files generated successfully!');
console.log(`   - ${provinces.length} provinces`);
console.log(`   - ${districts.length} districts`);
console.log(`   - ${neighborhoods.length} neighborhoods`);
