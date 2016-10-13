$maxRows = 100000
$minRows = 40000
$daysCount = 4
$filesPerDay = Get-Random -minimum 1 -maximum 5

$prods = @(
	@{ title="Halogen Headlights (2 Pack)"; cat="Lighting"; },
	@{ title="Bugeye Headlights (2 Pack)"; cat="Lighting"; },
	@{ title="Turn Signal Light Bulb"; cat="Lighting"; },
	@{ title="Matte Finish Rim"; cat="Wheels & Tires"; },
	@{ title="Blue Performance Alloy Rim"; cat="Wheels & Tires"; },
	@{ title="High Performance Rim"; cat="Wheels & Tires"; },
	@{ title="Wheel Tire Combo"; cat="Wheels & Tires"; },
	@{ title="Chrome Rim Tire Combo"; cat="Wheels & Tires"; },
	@{ title="Wheel Tire Combo (4 Pack)"; cat="Wheels & Tires"; },
	@{ title="Disk and Pad Combo"; cat="Wheels & Tires"; },
	@{ title="Brake Rotor"; cat="Brakes"; },
	@{ title="Brake Disk and Calipers"; cat="Brakes"; },
	@{ title="12-Volt Calcium Battery"; cat="Batteries"; },
	@{ title="Spiral Coil Battery"; cat="Batteries"; },
	@{ title="Jumper Leads"; cat="Batteries"; },
	@{ title="Filter Set"; cat="Oil"; },
	@{ title="Oil and Filter Combo"; cat="Oil"; },
	@{ title="Synthetic Engine Oil"; cat="Oil"; }
)

for($d = 0; $d -lt $daysCount; $d++) {
	$newdate = $(Get-Date (Get-Date).AddDays(-$d) -format yyyy-MM-dd)
	$folder = "Assets\logs\$(Get-Date (Get-Date).AddDays(-$d) -f yyyy\\MM\\dd)";
	Write-Host "Creating folder $folder..."
	New-Item -ItemType directory -Path $folder -Force
	
	for($i = 1; $i -le $filesPerDay; $i++) {
		$path = "$folder\data2$i.txt";

		$items = ""
		$totalRows = Get-Random -minimum $minRows -maximum $maxRows

		for($j = 1; $j -le $totalRows; $j++)
		{
			$p = Get-Random -minimum 0 -maximum $prods.length
			$items += "$($newdate)|$($p+1)|$($prods[$p].title)|$($prods[$p].cat)|$(("add", "view") | Get-Random)|$(Get-Random -minimum 0 -maximum 200)`r`n"

		}
		
		$items = $items.Substring(0, $items.length - 1) 
		
		Write-Host "Saving data into $path..."
		[System.IO.File]::WriteAllText($path, $items)
	}
}