# make sure you adjust this to point to the folder you want to monitor
$PathToMonitor = $PSScriptRoot + "\Assets\Graphics"

$FileSystemWatcher = New-Object System.IO.FileSystemWatcher
$FileSystemWatcher.Path  = $PathToMonitor
$FileSystemWatcher.IncludeSubdirectories = $true
$FileSystemWatcher.Filter = "*.psd"

# make sure the watcher emits events
$FileSystemWatcher.EnableRaisingEvents = $true

# define the code that should execute when a file change is detected
$Action = {
    $details = $event.SourceEventArgs
    $Name = $details.Name
    $FullPath = $details.FullPath
    $OldFullPath = $details.OldFullPath
    $OldName = $details.OldName
    $ChangeType = $details.ChangeType
    $Timestamp = $event.TimeGenerated
    $text = "{0} was {1} at {2}" -f $FullPath, $ChangeType, $Timestamp
    Write-Host ""
    Write-Host $text -ForegroundColor Green
    
    # you can also execute code based on change type here
    switch ($ChangeType)
    {
        'Changed' {  
        	$newName = $FullPath + ".psb"
        	$dirname = $FullPath | Split-Path
        	$fname = $Name | Split-Path -Leaf 
        	$fname = $fname.Split('.') | Select -First 1
        	$pname = $dirname+"\"+$fname
     
        	$exec = "convert " + $pname + ".psd " + $pname + ".psb"
        	Write-Host $exec
        	Invoke-Expression $exec
        	#convert $pname.psd $pname.psb
        }
        default { Write-Host $_ -ForegroundColor Red -BackgroundColor White }
    }
}

# add event handlers
$handlers = . {
    Register-ObjectEvent -InputObject $FileSystemWatcher -EventName Changed -Action $Action -SourceIdentifier FSChange
    #Register-ObjectEvent -InputObject $FileSystemWatcher -EventName Created -Action $Action -SourceIdentifier FSCreate
}

Write-Host "Watching for changes to $PathToMonitor"

try
{
    do
    {
        Wait-Event -Timeout 1
        Write-Host "." -NoNewline
        
    } while ($true)
}
finally
{
    # this gets executed when user presses CTRL+C
    # remove the event handlers
    Unregister-Event -SourceIdentifier FSChange
    Unregister-Event -SourceIdentifier FSCreate
    Unregister-Event -SourceIdentifier FSDelete
    Unregister-Event -SourceIdentifier FSRename
    # remove background jobs
    $handlers | Remove-Job
    # remove filesystemwatcher
    $FileSystemWatcher.EnableRaisingEvents = $false
    $FileSystemWatcher.Dispose()
    "Event Handler disabled."
}



