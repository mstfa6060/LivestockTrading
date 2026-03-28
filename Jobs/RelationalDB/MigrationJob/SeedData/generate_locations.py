#!/usr/bin/env python3
"""
GeoNames veri indirme ve provinces.json / districts.json olusturma scripti.

Kaynak: https://download.geonames.org/export/dump/
- admin1CodesASCII.txt: Il/Eyalet/Bolge (admin1 divisions)
- cities15000.zip: Nufusu 15000+ sehirler
- alternateNamesV2.zip: Coklu dil cevirileri

Cikti:
- provinces.json: Tum ulkelerin il/eyalet verileri
- districts.json: Tum sehirler (nufus 15000+)

Kullanim:
    pip install requests
    python generate_locations.py
"""

import json
import os
import sys
import zipfile
import io
import csv
from collections import defaultdict

try:
    import requests
except ImportError:
    print("ERROR: 'requests' paketi gerekli. Yukleyin: pip install requests")
    sys.exit(1)

# GeoNames URLs
ADMIN1_URL = "https://download.geonames.org/export/dump/admin1CodesASCII.txt"
CITIES_URL = "https://download.geonames.org/export/dump/cities15000.zip"
ALT_NAMES_URL = "https://download.geonames.org/export/dump/alternateNamesV2.zip"

# Cevirilecek diller
TARGET_LANGUAGES = {"en", "tr", "de", "fr", "es", "ar", "ru", "zh", "ja", "ko", "pt", "it", "nl", "pl", "hi", "fa", "ur", "az", "id", "th", "vi", "uk", "ro", "el", "he", "sv", "da", "no", "fi", "cs", "hu", "bg", "hr", "sr", "sk", "sl", "lt", "lv", "et", "ka", "hy", "ms", "tl", "sw", "bn", "ta", "te", "mr", "gu", "pa"}

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))


def load_country_mapping():
    """countries.json'dan ISO code → Country.Id eslesmesi yukle."""
    path = os.path.join(SCRIPT_DIR, "countries.json")
    with open(path, "r", encoding="utf-8") as f:
        countries = json.load(f)
    # ISO alpha-2 code → our DB Id
    return {c["Code"]: c["Id"] for c in countries}


def download_file(url, desc):
    """URL'den dosya indir, bytes dondur."""
    print(f"  Indiriliyor: {desc}...")
    resp = requests.get(url, stream=True, timeout=300)
    resp.raise_for_status()
    total = int(resp.headers.get("content-length", 0))
    data = bytearray()
    downloaded = 0
    for chunk in resp.iter_content(chunk_size=1024 * 256):
        data.extend(chunk)
        downloaded += len(chunk)
        if total > 0:
            pct = downloaded * 100 // total
            print(f"\r  {desc}: {pct}% ({downloaded // 1024 // 1024}MB / {total // 1024 // 1024}MB)", end="", flush=True)
    print()
    return bytes(data)


def parse_admin1(content, country_map):
    """
    admin1CodesASCII.txt parse et.
    Format: CC.ADMIN1_CODE\tName\tASCII Name\tGeoNameId
    """
    provinces = []
    for line in content.decode("utf-8").splitlines():
        parts = line.strip().split("\t")
        if len(parts) < 4:
            continue
        code_full = parts[0]  # e.g. "TR.34"
        name = parts[1]
        geoname_id = int(parts[3])

        cc, admin_code = code_full.split(".", 1)
        country_id = country_map.get(cc)
        if country_id is None:
            continue

        provinces.append({
            "country_code": cc,
            "country_id": country_id,
            "admin_code": admin_code,
            "name": name,
            "geoname_id": geoname_id,
            "code_full": code_full,
        })
    return provinces


def parse_cities(zip_data, country_map, admin1_lookup):
    """
    cities15000.txt parse et (zip icinden).
    GeoNames format (tab-separated):
    0:geonameid 1:name 2:asciiname 3:alternatenames 4:latitude 5:longitude
    6:feature_class 7:feature_code 8:country_code 9:cc2
    10:admin1_code 11:admin2_code 12:admin3_code 13:admin4_code
    14:population 15:elevation 16:dem 17:timezone 18:modification_date
    """
    districts = []
    with zipfile.ZipFile(io.BytesIO(zip_data)) as zf:
        for name in zf.namelist():
            if name.endswith(".txt"):
                with zf.open(name) as f:
                    for line in f:
                        parts = line.decode("utf-8").strip().split("\t")
                        if len(parts) < 15:
                            continue
                        geoname_id = int(parts[0])
                        city_name = parts[1]
                        latitude = float(parts[4]) if parts[4] else None
                        longitude = float(parts[5]) if parts[5] else None
                        cc = parts[8]
                        admin1_code = parts[10]
                        population = int(parts[14]) if parts[14] else 0

                        country_id = country_map.get(cc)
                        if country_id is None:
                            continue

                        # admin1 lookup key
                        admin1_key = f"{cc}.{admin1_code}"
                        province_info = admin1_lookup.get(admin1_key)
                        if province_info is None:
                            continue

                        districts.append({
                            "geoname_id": geoname_id,
                            "name": city_name,
                            "country_code": cc,
                            "admin1_key": admin1_key,
                            "population": population,
                            "latitude": latitude,
                            "longitude": longitude,
                        })
    return districts


def parse_alternate_names(zip_data, target_geoname_ids):
    """
    alternateNamesV2.zip parse et — sadece target_geoname_ids'e ait satirlari filtrele.
    Format (tab-separated):
    0:alternateNameId 1:geonameid 2:isolanguage 3:alternate_name
    4:isPreferredName 5:isShortName 6:isColloquial 7:isHistoric 8:from 9:to
    """
    translations = defaultdict(dict)  # geoname_id → {lang: name}

    print("  alternateNamesV2 parse ediliyor (buyuk dosya, biraz zaman alabilir)...")
    with zipfile.ZipFile(io.BytesIO(zip_data)) as zf:
        for zname in zf.namelist():
            if not zname.endswith(".txt"):
                continue
            with zf.open(zname) as f:
                line_count = 0
                for line in f:
                    line_count += 1
                    if line_count % 5_000_000 == 0:
                        print(f"\r  {line_count // 1_000_000}M satir islendi...", end="", flush=True)

                    parts = line.decode("utf-8", errors="replace").strip().split("\t")
                    if len(parts) < 4:
                        continue

                    gid = int(parts[1])
                    if gid not in target_geoname_ids:
                        continue

                    lang = parts[2]
                    if lang not in TARGET_LANGUAGES:
                        continue

                    alt_name = parts[3]
                    is_preferred = parts[4] == "1" if len(parts) > 4 else False
                    is_historic = parts[7] == "1" if len(parts) > 7 else False

                    if is_historic:
                        continue

                    # Preferred name tercih et, yoksa ilk geleni al
                    if lang not in translations[gid] or is_preferred:
                        translations[gid][lang] = alt_name

    print(f"\r  {line_count // 1_000_000}M satir islendi. Toplam {len(translations)} kayit icin ceviri bulundu.")
    return translations


def build_provinces_json(raw_provinces, translations):
    """provinces.json formatinda JSON olustur."""
    # Ulke bazli siralama
    raw_provinces.sort(key=lambda p: (p["country_id"], p["name"]))

    result = []
    # Her ulke icin sequential ID: country_id * 10000 + seq
    country_seq = defaultdict(int)

    for p in raw_provinces:
        cid = p["country_id"]
        country_seq[cid] += 1
        seq = country_seq[cid]
        province_id = cid * 10000 + seq

        name_trans = translations.get(p["geoname_id"], {})
        # JSON string olarak kaydet
        name_trans_json = json.dumps(name_trans, ensure_ascii=False) if name_trans else None

        result.append({
            "Id": province_id,
            "CountryId": cid,
            "Name": p["name"],
            "Code": p["admin_code"],
            "NameTranslations": name_trans_json,
            "GeoNameId": p["geoname_id"],
            "SortOrder": seq,
        })

    return result


def build_districts_json(raw_districts, provinces_json, admin1_lookup, translations):
    """districts.json formatinda JSON olustur."""
    # Province code_full → Province.Id eslesmesi
    province_id_map = {}
    for p in provinces_json:
        # code_full'u bulmak icin admin1_lookup'a geri bakalim
        pass

    # admin1_key → province_id eslesmesi
    admin1_to_province_id = {}
    for p_raw in admin1_lookup.values():
        # provinces_json'dan bul
        pass

    # Daha temiz yaklasim: raw_provinces bilgilerini kullan
    # admin1_key → province_json_id
    code_to_province = {}
    for pj in provinces_json:
        # GeoNameId ile esle
        code_to_province[pj["GeoNameId"]] = pj["Id"]

    # admin1_lookup: code_full → raw_province (geoname_id iceriyor)
    admin1_key_to_province_id = {}
    for key, info in admin1_lookup.items():
        gid = info["geoname_id"]
        if gid in code_to_province:
            admin1_key_to_province_id[key] = code_to_province[gid]

    # Siralama: province_id → population DESC
    raw_districts.sort(key=lambda d: (admin1_key_to_province_id.get(d["admin1_key"], 0), -d["population"]))

    result = []
    province_seq = defaultdict(int)

    for d in raw_districts:
        province_id = admin1_key_to_province_id.get(d["admin1_key"])
        if province_id is None:
            continue

        province_seq[province_id] += 1
        seq = province_seq[province_id]

        # District ID: province_id * 1000 + seq
        # Max: 1960000 * 1000 + 999 = 1,960,000,999 (int.MaxValue altinda)
        district_id = province_id * 1000 + seq

        if district_id > 2_000_000_000:
            print(f"  UYARI: District ID tasmasi! province_id={province_id}, seq={seq}")
            continue

        name_trans = translations.get(d["geoname_id"], {})
        name_trans_json = json.dumps(name_trans, ensure_ascii=False) if name_trans else None

        result.append({
            "Id": district_id,
            "ProvinceId": province_id,
            "Name": d["name"],
            "NameTranslations": name_trans_json,
            "GeoNameId": d["geoname_id"],
            "Latitude": d.get("latitude"),
            "Longitude": d.get("longitude"),
            "SortOrder": seq,
        })

    return result


def main():
    print("=" * 60)
    print("GeoNames Location Data Generator")
    print("=" * 60)

    # 1. Country mapping yukle
    print("\n[1/6] Country mapping yukleniyor...")
    country_map = load_country_mapping()
    print(f"  {len(country_map)} ulke eslesmesi yuklendi.")

    # 2. admin1 indir ve parse et
    print("\n[2/6] Admin1 (il/eyalet) verileri indiriliyor...")
    admin1_data = download_file(ADMIN1_URL, "admin1CodesASCII.txt")
    raw_provinces = parse_admin1(admin1_data, country_map)
    print(f"  {len(raw_provinces)} il/eyalet parse edildi.")

    # admin1 lookup olustur
    admin1_lookup = {p["code_full"]: p for p in raw_provinces}

    # 3. cities15000 indir ve parse et
    print("\n[3/6] Cities15000 (sehir) verileri indiriliyor...")
    cities_data = download_file(CITIES_URL, "cities15000.zip")
    raw_districts = parse_cities(cities_data, country_map, admin1_lookup)
    print(f"  {len(raw_districts)} sehir parse edildi.")

    # 4. Cevirileri indir
    print("\n[4/6] Alternate names (ceviriler) indiriliyor...")
    # Tum geoname_id'leri topla
    all_geoname_ids = set()
    for p in raw_provinces:
        all_geoname_ids.add(p["geoname_id"])
    for d in raw_districts:
        all_geoname_ids.add(d["geoname_id"])
    print(f"  {len(all_geoname_ids)} benzersiz GeoNameId icin ceviri aranacak.")

    alt_names_data = download_file(ALT_NAMES_URL, "alternateNamesV2.zip")
    translations = parse_alternate_names(alt_names_data, all_geoname_ids)

    # 5. JSON olustur
    print("\n[5/6] JSON dosyalari olusturuluyor...")
    provinces_json = build_provinces_json(raw_provinces, translations)
    districts_json = build_districts_json(raw_districts, provinces_json, admin1_lookup, translations)

    # 6. Dosyalara yaz
    print("\n[6/6] Dosyalar yaziliyor...")
    provinces_path = os.path.join(SCRIPT_DIR, "provinces.json")
    districts_path = os.path.join(SCRIPT_DIR, "districts.json")

    with open(provinces_path, "w", encoding="utf-8") as f:
        json.dump(provinces_json, f, ensure_ascii=False, indent=2)
    print(f"  provinces.json: {len(provinces_json)} kayit ({os.path.getsize(provinces_path) // 1024}KB)")

    with open(districts_path, "w", encoding="utf-8") as f:
        json.dump(districts_json, f, ensure_ascii=False, indent=2)
    print(f"  districts.json: {len(districts_json)} kayit ({os.path.getsize(districts_path) // 1024}KB)")

    # Ozet istatistikler
    countries_with_provinces = len(set(p["CountryId"] for p in provinces_json))
    print(f"\n{'=' * 60}")
    print(f"OZET:")
    print(f"  Ulke sayisi (il verisi olan): {countries_with_provinces}")
    print(f"  Toplam il/eyalet: {len(provinces_json)}")
    print(f"  Toplam sehir/ilce: {len(districts_json)}")
    print(f"  Ceviri bulunan kayit: {sum(1 for p in provinces_json if p['NameTranslations'])} province + {sum(1 for d in districts_json if d['NameTranslations'])} district")
    print(f"{'=' * 60}")


if __name__ == "__main__":
    main()
