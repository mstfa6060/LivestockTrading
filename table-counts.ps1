$connectionString = "Server=45.143.4.64,1440;Database=livestocktrading_dev;User Id=sa;Password=B7o5zctnHaxCtQp5u2kL;Encrypt=True;TrustServerCertificate=True;"

$query = @"
SELECT
    t.NAME AS TableName,
    p.rows AS RecordCount
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id
WHERE p.index_id IN (0, 1)
    AND t.is_ms_shipped = 0
ORDER BY p.rows DESC, t.NAME
"@

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()

    $command = New-Object System.Data.SqlClient.SqlCommand($query, $connection)
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
    $dataset = New-Object System.Data.DataSet
    $adapter.Fill($dataset) | Out-Null

    Write-Host "`n========================================" -ForegroundColor Yellow
    Write-Host "TABLO KAYIT SAYILARI" -ForegroundColor Yellow
    Write-Host "========================================`n" -ForegroundColor Yellow

    $totalRows = 0
    $tableCount = 0

    foreach ($row in $dataset.Tables[0].Rows) {
        $tableName = $row.TableName
        $rowCount = $row.RecordCount
        $totalRows += $rowCount
        $tableCount++

        $color = if ($rowCount -gt 0) { "Green" } else { "Gray" }
        Write-Host ("{0,-40} {1,10:N0}" -f $tableName, $rowCount) -ForegroundColor $color
    }

    Write-Host "`n----------------------------------------" -ForegroundColor Yellow
    Write-Host ("{0,-40} {1,10:N0}" -f "TOPLAM ($tableCount tablo)", $totalRows) -ForegroundColor Cyan

    $connection.Close()
}
catch {
    Write-Host "Hata: $($_.Exception.Message)" -ForegroundColor Red
}
