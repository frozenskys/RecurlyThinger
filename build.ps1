rm .\output\ -Recurse -Force -ErrorAction SilentlyContinue
rm .\SubscriptionThinger.zip -Recurse -Force -ErrorAction SilentlyContinue
dotnet publish -o output
Rename-Item -Path .\output\RecurlyThinger.exe -NewName SubscriptionThinger.exe
Compress-Archive -Path .\output\SubscriptionThinger.exe -DestinationPath SubscriptionThinger.zip
rm .\output\ -Recurse -Force -ErrorAction SilentlyContinue