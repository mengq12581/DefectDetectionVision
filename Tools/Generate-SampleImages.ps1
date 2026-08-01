[CmdletBinding()]
param(
    [string]$OutputFolder = "",
    [int]$ImageCount = 8,
    [int]$Seed = 20260730
)

Add-Type -AssemblyName System.Drawing

$scriptFolder = Split-Path -Parent $MyInvocation.MyCommand.Path
if ([string]::IsNullOrWhiteSpace($OutputFolder))
{
    $OutputFolder = Join-Path $scriptFolder "..\VisionDemo2026\SampleImages"
}

$imageWidth = 1080
$imageHeight = 720
$backgroundColor = [System.Drawing.Color]::FromArgb(214, 218, 222)
$random = [System.Random]::new($Seed)
$manifest = [System.Collections.Generic.List[object]]::new()

function Get-ShuffledItems
{
    param(
        [object[]]$Items,
        [System.Random]$Random
    )

    $result = [object[]]$Items.Clone()
    for ($i = $result.Length - 1; $i -gt 0; $i--)
    {
        $swapIndex = $Random.Next(0, $i + 1)
        $temporary = $result[$i]
        $result[$i] = $result[$swapIndex]
        $result[$swapIndex] = $temporary
    }
    return $result
}

function Get-PolarPoint
{
    param(
        [double]$CenterX,
        [double]$CenterY,
        [double]$Radius,
        [double]$AngleDegree
    )

    $angleRadian = $AngleDegree * [Math]::PI / 180.0
    return [System.Drawing.PointF]::new(
        [float]($CenterX + $Radius * [Math]::Cos($angleRadian)),
        [float]($CenterY + $Radius * [Math]::Sin($angleRadian)))
}

function Draw-Ring
{
    param(
        [System.Drawing.Graphics]$Graphics,
        [System.Drawing.Brush]$PartBrush,
        [System.Drawing.Brush]$BackgroundBrush,
        [double]$CenterX,
        [double]$CenterY,
        [double]$OuterDiameter,
        [double]$InnerDiameter
    )

    $outerRadius = $OuterDiameter / 2.0
    $innerRadius = $InnerDiameter / 2.0
    $Graphics.FillEllipse($PartBrush,
        [float]($CenterX - $outerRadius), [float]($CenterY - $outerRadius),
        [float]$OuterDiameter, [float]$OuterDiameter)
    $Graphics.FillEllipse($BackgroundBrush,
        [float]($CenterX - $innerRadius), [float]($CenterY - $innerRadius),
        [float]$InnerDiameter, [float]$InnerDiameter)
}

function Draw-FracturedRing
{
    param(
        [System.Drawing.Graphics]$Graphics,
        [System.Drawing.Brush]$PartBrush,
        [System.Drawing.Brush]$BackgroundBrush,
        [double]$CenterX,
        [double]$CenterY,
        [double]$OuterDiameter,
        [double]$InnerDiameter,
        [System.Random]$Random
    )

    Draw-Ring $Graphics $PartBrush $BackgroundBrush $CenterX $CenterY $OuterDiameter $InnerDiameter

    # 裂缝从外圆贯穿到内孔，外接矩形尺寸仍与合格圆环基本一致。
    $angle = $Random.Next(25, 336)
    $halfWidth = $Random.Next(9, 15)
    $outerRadius = $OuterDiameter / 2.0 + 3
    $innerRadius = [Math]::Max(1, $InnerDiameter / 2.0 - 3)
    $fracturePoints = [System.Drawing.PointF[]]@(
        (Get-PolarPoint $CenterX $CenterY $outerRadius ($angle - $halfWidth)),
        (Get-PolarPoint $CenterX $CenterY $outerRadius ($angle + $halfWidth)),
        (Get-PolarPoint $CenterX $CenterY $innerRadius ($angle + $halfWidth)),
        (Get-PolarPoint $CenterX $CenterY $innerRadius ($angle - $halfWidth))
    )
    $Graphics.FillPolygon($BackgroundBrush, $fracturePoints)

    # 部分缺陷增加短分支，使裂缝形态不完全相同。
    if ($Random.NextDouble() -lt 0.45)
    {
        $branchStart = Get-PolarPoint $CenterX $CenterY (($outerRadius + $innerRadius) / 2.0) $angle
        $branchEnd = Get-PolarPoint $branchStart.X $branchStart.Y $Random.Next(10, 18) ($angle + $Random.Next(35, 66))
        $branchPen = [System.Drawing.Pen]::new($backgroundColor, $Random.Next(3, 6))
        try
        {
            $branchPen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
            $branchPen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
            $Graphics.DrawLine($branchPen, $branchStart, $branchEnd)
        }
        finally
        {
            $branchPen.Dispose()
        }
    }
}

function Draw-DeformedRing
{
    param(
        [System.Drawing.Graphics]$Graphics,
        [System.Drawing.Brush]$PartBrush,
        [System.Drawing.Brush]$BackgroundBrush,
        [double]$CenterX,
        [double]$CenterY,
        [System.Random]$Random
    )

    # 改变长短轴但保持环形面积接近合格件，用于突出形状异常而非尺寸异常。
    $stretch = $Random.Next(-4, 5)
    $outerWidth = 136 + $stretch
    $outerHeight = 59 - [Math]::Round($stretch * 0.4)
    $innerWidth = 46
    $innerHeight = 24
    $rotation = $Random.Next(-55, 56)
    $graphicsState = $Graphics.Save()
    try
    {
        $Graphics.TranslateTransform([float]$CenterX, [float]$CenterY)
        $Graphics.RotateTransform([float]$rotation)
        $Graphics.FillEllipse($PartBrush,
            [float](-$outerWidth / 2.0), [float](-$outerHeight / 2.0),
            [float]$outerWidth, [float]$outerHeight)
        $Graphics.FillEllipse($BackgroundBrush,
            [float](-$innerWidth / 2.0), [float](-$innerHeight / 2.0),
            [float]$innerWidth, [float]$innerHeight)
    }
    finally
    {
        $Graphics.Restore($graphicsState)
    }
}

$resolvedOutputFolder = [System.IO.Path]::GetFullPath($OutputFolder)
[System.IO.Directory]::CreateDirectory($resolvedOutputFolder) | Out-Null

for ($imageIndex = 1; $imageIndex -le $ImageCount; $imageIndex++)
{
    $bitmap = [System.Drawing.Bitmap]::new(
        $imageWidth, $imageHeight, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $backgroundBrush = [System.Drawing.SolidBrush]::new($backgroundColor)
    try
    {
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $graphics.FillRectangle($backgroundBrush, 0, 0, $imageWidth, $imageHeight)

        # 先绘制少量微小噪点，后续零件不会被噪点切断。
        $noiseCount = $random.Next(90, 151)
        for ($noiseIndex = 0; $noiseIndex -lt $noiseCount; $noiseIndex++)
        {
            $noiseGray = $random.Next(90, 191)
            $noiseBrush = [System.Drawing.SolidBrush]::new(
                [System.Drawing.Color]::FromArgb($noiseGray, $noiseGray, $noiseGray))
            try
            {
                $noiseSize = $random.Next(1, 4)
                $graphics.FillEllipse($noiseBrush,
                    $random.Next(8, $imageWidth - 8), $random.Next(8, $imageHeight - 8),
                    $noiseSize, $noiseSize)
            }
            finally
            {
                $noiseBrush.Dispose()
            }
        }

        $partCount = $random.Next(15, 21)
        $fractureCount = $random.Next(3, 5)
        $deformedCount = $random.Next(3, 5)
        $qualifiedCount = $partCount - $fractureCount - $deformedCount

        $categories = [System.Collections.Generic.List[string]]::new()
        for ($i = 0; $i -lt $qualifiedCount; $i++) { $categories.Add("合格圆环") }
        for ($i = 0; $i -lt $fractureCount; $i++) { $categories.Add("裂缝或断裂") }
        for ($i = 0; $i -lt $deformedCount; $i++) { $categories.Add("变形") }
        $shuffledCategories = Get-ShuffledItems $categories.ToArray() $random

        # 从 6×4 个安全工位中随机选取，加入有限扰动后仍可保证不裁切、不叠放。
        $candidateCells = [System.Collections.Generic.List[object]]::new()
        for ($row = 0; $row -lt 4; $row++)
        {
            for ($column = 0; $column -lt 6; $column++)
            {
                $candidateCells.Add([PSCustomObject]@{
                    X = 100 + $column * 176
                    Y = 100 + $row * 170
                })
            }
        }
        $shuffledCells = Get-ShuffledItems $candidateCells.ToArray() $random

        for ($partIndex = 0; $partIndex -lt $partCount; $partIndex++)
        {
            $centerX = $shuffledCells[$partIndex].X + $random.Next(-14, 15)
            $centerY = $shuffledCells[$partIndex].Y + $random.Next(-14, 15)
            $category = $shuffledCategories[$partIndex]
            $partGray = $random.Next(42, 56)
            $partBrush = [System.Drawing.SolidBrush]::new(
                [System.Drawing.Color]::FromArgb($partGray, $partGray, $partGray))
            try
            {
                $diameterOffset = $random.Next(-1, 2)
                $outerDiameter = 90 + $diameterOffset
                $innerDiameter = 36 + $diameterOffset
                if ($category -eq "合格圆环")
                {
                    Draw-Ring $graphics $partBrush $backgroundBrush $centerX $centerY $outerDiameter $innerDiameter
                }
                elseif ($category -eq "裂缝或断裂")
                {
                    Draw-FracturedRing $graphics $partBrush $backgroundBrush $centerX $centerY `
                        $outerDiameter $innerDiameter $random
                }
                else
                {
                    Draw-DeformedRing $graphics $partBrush $backgroundBrush $centerX $centerY $random
                }
            }
            finally
            {
                $partBrush.Dispose()
            }

            $manifest.Add([PSCustomObject]@{
                Image = "sample_{0:D2}.bmp" -f $imageIndex
                Index = $partIndex + 1
                Category = $category
                CenterX = $centerX
                CenterY = $centerY
            })
        }

        $imagePath = Join-Path $resolvedOutputFolder ("sample_{0:D2}.bmp" -f $imageIndex)
        $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Bmp)
    }
    finally
    {
        $backgroundBrush.Dispose()
        $graphics.Dispose()
        $bitmap.Dispose()
    }
}

$manifestPath = Join-Path $resolvedOutputFolder "sample_manifest.csv"
$manifest | Export-Csv -LiteralPath $manifestPath -NoTypeInformation -Encoding UTF8
Write-Output ("已生成 {0} 张样例图：{1}" -f $ImageCount, $resolvedOutputFolder)
