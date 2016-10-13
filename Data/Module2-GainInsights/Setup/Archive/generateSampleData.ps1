$maxRows = 15
$minRows = 5
$daysCount = 4
$hoursCount = 24
$minutesCount = 60
$filesPerDay = Get-Random -minimum 3 -maximum 6

$prods = @(
            
            @{
                SkuNumber = "LIG-0001";
                Title = "Halogen Headlights (2 Pack)";
                Price = 38.99;
                SalePrice = 38.99;
                CostPrice = 16.99;
                Description = "Our Halogen Headlights are made to fit majority of vehicles with our  universal fitting mold. Product requires some assembly.";
                ProductId = 1;
            },

            
            @{
                SkuNumber = "LIG-0002";
                Title = "Bugeye Headlights (2 Pack)";
                Price = 48.99;
                SalePrice = 48.99;
                CostPrice = 21.00;
                Description = "Our Bugeye Headlights use Halogen light bulbs are made to fit into a standard bugeye slot. Product requires some assembly and includes light bulbs.";
                ProductId = 2;
            },

            
            @{
                SkuNumber = "LIG-0003";
                Title = "Turn Signal Light Bulb";
                Price = 6.49;
                SalePrice = 6.49;
                CostPrice = 3.99;
                Description = " Clear bulb that with a universal fitting for all headlights/taillights.  Simple Installation, low wattage and a clear light for optimal visibility and efficiency.";
                ProductId = 3;
            },

            
            @{
                SkuNumber = "WHE-0001";
                Title = "Matte Finish Rim";
                Price = 75.99;
                SalePrice = 75.99;
                CostPrice = 39.00;
                Description = "A Parts Unlimited favorite, the Matte Finish Rim is affordable low profile style. Fits all low profile tires.";
                ProductId = 4;
            },

            
            @{
                SkuNumber = "WHE-0002";
                Title = "Blue Performance Alloy Rim";
                Price = 88.99;
                SalePrice = 88.99;
                CostPrice = 34.50;
                Description = "Stand out from the crowd with a set of aftermarket blue rims to make you vehicle turn heads and at a price that will do the same.";
                ProductId = 5;
            },

            
            @{
                SkuNumber = "WHE-0003";
                Title = "High Performance Rim";
                Price = 99.99;
                SalePrice = 99.49;
                CostPrice = 74.60;
                Description = "Light Weight Rims with a twin cross spoke design for stability and reliable performance.";
                ProductId = 6;
            },

            
            @{
                SkuNumber = "WHE-0004";
                Title = "Wheel Tire Combo";
                Price = 72.49;
                SalePrice = 72.49;
                CostPrice = 32.30;
                Description = "For the endurance driver, take advantage of our best wearing tire yet. Composite rubber and a heavy duty steel rim.";
                ProductId = 7;
            },

            
            @{
                SkuNumber = "WHE-0005";
                Title = "Chrome Rim Tire Combo";
                Price = 129.99;
                SalePrice = 129.99;
                CostPrice = 47.10;
                Description = "Save time and money with our ever popular wheel and tire combo. Pre-assembled and ready to go.";
                ProductId = 8;
            },

            
            @{
                SkuNumber = "WHE-0006";
                Title = "Wheel Tire Combo (4 Pack)";
                Price = 219.99;
                SalePrice = 219.99;
                CostPrice = 89.40;
                Description = "Having trouble in the wet? Then try our special patent tire on a heavy duty steel rim. These wheels perform excellent in all conditions but were designed specifically for wet weather.";
                ProductId = 9;
            },

            
            @{
                SkuNumber = "BRA-0001";
                Title = "Disk and Pad Combo";
                Price = 25.99;
                SalePrice = 25.99;
                CostPrice = 16.40;
                Description = "Our brake disks and pads perform the best togeather. Better stopping distances without locking up, reduced rust and dusk.";
                ProductId = 10;
            },

            
            @{
                SkuNumber = "BRA-0002";
                Title = "Brake Rotor";
                Price = 18.99;
                SalePrice = 18.99;
                CostPrice = 7.99;
                Description = "Our Brake Rotor Performs well in wet coditions with a smooth responsive feel. Machined to a high tolerance to ensure all of our Brake Rotors are safe and reliable.";
                ProductId = 11;
            },

            
            @{
                SkuNumber = "BRA-0003";
                Title = "Brake Disk and Calipers";
                Price = 43.99;
                SalePrice = 43.99;
                CostPrice = 38.10;
                Description = "Upgrading your brakes can increase stopping power, reduce dust and noise. Our Disk Calipers exceed factory specification for the best performance.";
                ProductId = 12;
            },

            
            @{
                SkuNumber = "BAT-0001";
                Title = "12-Volt Calcium Battery";
                Price = 129.99;
                SalePrice = 129.99;
                CostPrice = 80.00;
                Description = "Calcium is the most common battery type. It is durable and has a long shelf and service life. They also provide high cold cranking amps.";
                ProductId = 13;
            },

            
            @{
                SkuNumber = "BAT-0002";
                Title = "Spiral Coil Battery";
                Price = 154.99;
                SalePrice = 154.99;
                CostPrice = 99.10;
                Description = "Spiral Coil batteries are the preferred option for high performance Vehicles where extra toque is need for starting. They are more resistant to heat and higher charge rates than conventional batteries.";
                ProductId = 14;
            },

            
            @{
                SkuNumber = "BAT-0003";
                Title = "Jumper Leads";
                Price = 16.99;
                SalePrice = 16.99;
                CostPrice = 7.10;
                Description = "Battery Jumper Leads have a built in surge protector and a includes a plastic carry case to keep them safe from corrosion.";
                ProductId = 15;
            },

            
            @{
                SkuNumber = "OIL-0001";
                Title = "Filter Set";
                Price = 28.99;
                SalePrice = 28.99;
                CostPrice = 23.99;
                Description = "Ensure that your vehicle's engine has a longer life with our new filter set. Trapping more dirt to ensure old freely circulates through your engine.";
                ProductId = 16;
            },

            
            @{
                SkuNumber = "OIL-0002";
                Title = "Oil and Filter Combo";
                Price = 34.49;
                SalePrice = 34.49;
                CostPrice = 19.40;
                Description = "This Oil and Oil Filter combo is suitable for all types of passenger and light commercial vehicles. Providing affordable performance through excellent lubrication and breakdown resistance.";
                ProductId = 17;
            },

            
            @{
                SkuNumber = "OIL-0003";
                Title = "Synthetic Engine Oil";
                Price = 36.49;
                SalePrice = 36.49;
                CostPrice = 22.10;
                Description = "This Oil is designed to reduce sludge deposits and metal friction throughout your cars engine. Provides performance no matter the condition or temperature.";
                ProductId = 18;
            }
)

for($d = 0; $d -lt $daysCount; $d++) {
	$newdate = $(Get-Date (Get-Date).AddDays(-$d) -format yyyy-MM-dd)
	$folder = "Assets\logs\$(Get-Date (Get-Date).AddDays(-$d) -f yyyy\\MM\\dd)";
	Write-Host "Creating folder $folder..."
	New-Item -ItemType directory -Path $folder -Force
	
	for($i = 1; $i -le $filesPerDay; $i++) {
		$path = "$folder\data$i.txt";

		$items = ""

		for($j = 0; $j -lt $hoursCount; $j++)
		{
            for($k = 0 ; $k -lt $minutesCount; $k++)
            {
                
		        $totalRows = Get-Random -minimum $minRows -maximum $maxRows
                
                for($h = 1 ; $h -le $totalRows ; $h ++)
                {
                    $p = Get-Random -minimum 0 -maximum ($prods.length)
                    $u = Get-Random -minimum 1 -maximum 250
                    $qty = Get-Random -Minimum 1 -Maximum 4
                    $sec = (60*$h / $totalRows)-1
                    $type = $(("add", "view", "checkout", "remove") | Get-Random)
                    $eventDate = $newdate+"T"+ $j.ToString("00") + ":" + $k.ToString("00") + ":" + $sec.ToString("00") + ".0000000Z"

                    if($type -eq 'checkout')
                    {
                        $items += "{""eventDate"":""$($eventDate)"",""userId"":""$($u)"",""type"":""$type"",""productId"":""$($p+1)"",""quantity"":"+($qty -as [int])+",""price"":"+($($prods[$p].SalePrice)*$qty -as [decimal])+"}`r`n"
                    }
                    else
                    {
                        $items += "{""eventDate"":""$($eventDate)"",""userId"":""$($u)"",""type"":""$type"",""productId"":""$($p+1)"",""quantity"":"+(1 -as [int])+",""price"":"+(0.00 -as [decimal])+"}`r`n"
                    }   
                }
			    
            }
            
		}
		
		$items = $items.Substring(0, $items.length - 1) 
		
		Write-Host "Saving data into $path..."
		[System.IO.File]::WriteAllText($path, $items)
	}
}