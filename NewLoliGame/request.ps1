## 然后执行：.\http.ps1 -target "http://127.0.0.1:8088/login" -verb "POST" -content '{"account":"admin","password":"123456"}'

[CmdletBinding()]

Param(
    [Parameter(Mandatory=$True,Position=1)]
    [string]    $target,

    [Parameter(Mandatory=$True,Position=2)]

    [string]    $verb,      

    [Parameter(Mandatory=$True,Position=3)]
    [string]    $content
)

write-host "Http Url: $target"

write-host "Http Verb: $verb"

write-host "Http Content: $content"

$webRequest = [System.Net.WebRequest]::Create($target)

$encodedContent = [System.Text.Encoding]::UTF8.GetBytes($content)

$webRequest.Method = $verb

write-host "UTF8 Encoded Http Content: $content"


$webRequest.ContentType = "application/json"
$webRequest.ContentLength = $encodedContent.length
$requestStream = $webRequest.GetRequestStream()
$requestStream.Write($encodedContent, 0, $encodedContent.length)
$requestStream.Close()


[System.Net.WebResponse] $resp = $webRequest.GetResponse();

if($resp -ne $null) 

{
$rs = $resp.GetResponseStream();
[System.IO.StreamReader] $sr = New-Object System.IO.StreamReader -argumentList $rs;
[string] $results = $sr.ReadToEnd();
return $results

}

else

{
exit ''

}