#!/bin/bash
set -e

MINIO_CTR="livestock_prod-minio-1"
SQL_CTR="livestock_prod-sqlserver-1"
SA_PASS='LivestockProd@VeryStrong2025!Secure'
BUCKET="livestocktrading-production"

docker exec $MINIO_CTR mc alias set myminio http://localhost:9000 livestocktrading_admin 'LivestockMinIO@Prod2025!VerySecure' 2>/dev/null

echo "=== Fetching product list ==="
PRODUCTS=$(docker exec $SQL_CTR /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASS" -C \
  -d livestocktrading_production -h -1 -W -s'|' \
  -Q "SET NOCOUNT ON; SELECT p.Id, p.Slug, c.Slug AS CatSlug FROM Products p JOIN Categories c ON c.Id=p.CategoryId WHERE p.IsDeleted=0 AND p.Status=2 ORDER BY p.CreatedAt" \
  | grep -v "^$" | grep -v "rows affected")

TOTAL=$(echo "$PRODUCTS" | wc -l)
echo "Found $TOTAL products"
rm -f /tmp/uploaded_pids.txt
touch /tmp/uploaded_pids.txt

COUNT=0
echo "$PRODUCTS" | while IFS='|' read -r PID PSLUG CATSLUG; do
  [ -z "$PID" ] && continue
  COUNT=$((COUNT + 1))

  case "$CATSLUG" in
    dairy-cattle|holstein-friesian|jersey|brown-swiss|simmental|montbeliarde|normande|ayrshire|guernsey) S="dairy,cow" ;;
    beef-cattle|angus|hereford|charolais|limousin|brahman|wagyu|belgian-blue|shorthorn|highland|piedmontese) S="beef,cattle" ;;
    breeding-bulls) S="bull,farm" ;;
    calves) S="calf,cow" ;;
    heifers) S="heifer,cow" ;;
    buffalo|anatolian-buffalo|murrah-buffalo|*-buffalo) S="buffalo,water" ;;
    sheep|akkaraman|merinos|suffolk*|dorper|texel|hampshire*|kivircik|morkaraman|daglic|sakiz*|ivesi|karacabey*|lacaune|awassi|ile-de-france*) S="sheep,flock" ;;
    lambs|*-lamb) S="lamb,sheep" ;;
    breeding-rams) S="ram,sheep" ;;
    goats|saanen|alpine*|boer*|hair-goat|kilis*|damascus*|nubian*|toggenburg|angora-goat|lamancha*|murciana*|maltese*) S="goat,farm" ;;
    kids*|*-kid) S="baby,goat" ;;
    breeding-bucks) S="goat,billy" ;;
    laying-hens|leghorn|rhode-island*|plymouth*|sussex*|marans|ameraucana|isa-brown|denizli*) S="chicken,hen" ;;
    broilers|ross-308|cobb*|hubbard*|arbor*|sasso*) S="chicken,poultry" ;;
    turkeys|*-turkey) S="turkey,bird" ;;
    ducks*|*-duck|*-goose|toulouse*|embden*|chinese*) S="duck,goose" ;;
    quail|*-quail) S="quail,bird" ;;
    chicks) S="chick,baby" ;;
    ostrich|*-ostrich|emu|rhea) S="ostrich,bird" ;;
    horses|arabian*|thoroughbred|quarter*|friesian*|appaloosa|clydesdale|hanoverian|anatolian-horse|uzunyayla*|haflinger) S="horse,equine" ;;
    donkeys|*-donkey) S="donkey,animal" ;;
    mules) S="mule,animal" ;;
    camels|*-camel) S="camel,desert" ;;
    *-dog|kangal*|akbash*|anatolian-shepherd|border-collie|australian-shepherd|kuvasz*) S="shepherd,dog" ;;
    rabbits|*-rabbit) S="rabbit,bunny" ;;
    *trout) S="trout,fish" ;;
    *-bream) S="fish,seafood" ;;
    *-bass) S="fish,seafood" ;;
    fish|carp|tilapia|catfish|salmon|sturgeon) S="fish,farm" ;;
    fingerlings|*-fingerlings) S="fish,small" ;;
    beehives) S="beehive,apiary" ;;
    queen-bees) S="bee,queen" ;;
    honey*) S="honey,jar" ;;
    beekeeping*) S="beekeeper,hive" ;;
    tractors) S="tractor,field" ;;
    milking*) S="milking,dairy" ;;
    irrigation*) S="irrigation,farm" ;;
    livestock-equipment) S="cattle,equipment" ;;
    harvesting*) S="harvester,combine" ;;
    plowing*) S="plow,tractor" ;;
    trailers*) S="trailer,truck" ;;
    animal-feed) S="feed,grain" ;;
    hay*) S="hay,bales" ;;
    silage) S="corn,silage" ;;
    feed-supplements) S="supplement,animal" ;;
    alfalfa) S="alfalfa,field" ;;
    bran*) S="wheat,grain" ;;
    veterinary*|vaccines|antiparasitic*|health-supplements) S="veterinary,medicine" ;;
    field-crops) S="corn,seeds" ;;
    vegetable*) S="vegetable,seeds" ;;
    fruit*) S="fruit,orchard" ;;
    pasture*) S="pasture,grass" ;;
    fencing*) S="fence,farm" ;;
    barns*) S="barn,farm" ;;
    waterers*) S="trough,water" ;;
    fertilizers*) S="fertilizer,soil" ;;
    *) S="farm,agriculture" ;;
  esac

  TMPFILE="/tmp/img_${COUNT}.jpg"
  echo "[$COUNT/$TOTAL] $PSLUG ($S)"

  if curl -sL -o "$TMPFILE" --max-time 20 "https://loremflickr.com/640/480/${S}" 2>/dev/null; then
    FSIZE=$(stat -c%s "$TMPFILE" 2>/dev/null || echo "0")
    if [ "$FSIZE" -gt 5000 ]; then
      docker cp "$TMPFILE" "${MINIO_CTR}:/tmp/cover.jpg"
      docker exec $MINIO_CTR mc cp "/tmp/cover.jpg" "myminio/${BUCKET}/${PID}/cover.jpg" 2>/dev/null
      docker exec $MINIO_CTR rm -f "/tmp/cover.jpg"
      echo "  OK (${FSIZE} bytes)"
      echo "${PID}" >> /tmp/uploaded_pids.txt
    else
      echo "  SKIP (${FSIZE} bytes)"
    fi
    rm -f "$TMPFILE"
  else
    echo "  FAIL"
  fi
  sleep 1
done

echo ""
echo "=== SQL Update ==="
echo "BEGIN TRANSACTION;" > /tmp/update_images.sql
while read -r PID; do
  echo "UPDATE Products SET MediaBucketId='${PID}', CoverImageFileId='cover.jpg', UpdatedAt=GETUTCDATE() WHERE Id='${PID}';" >> /tmp/update_images.sql
done < /tmp/uploaded_pids.txt
echo "SELECT COUNT(*) AS ImagesSet FROM Products WHERE CoverImageFileId IS NOT NULL AND CoverImageFileId != '' AND IsDeleted=0;" >> /tmp/update_images.sql
echo "COMMIT TRANSACTION;" >> /tmp/update_images.sql
echo "GO" >> /tmp/update_images.sql

docker cp /tmp/update_images.sql "${SQL_CTR}:/tmp/update_images.sql"
docker exec $SQL_CTR /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASS" -C \
  -d livestocktrading_production -i /tmp/update_images.sql

echo "=== DONE ==="
