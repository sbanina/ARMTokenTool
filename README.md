ARMTokenTool
============

Tool to get JWT to call ARM (Azure Resource Manager)

Overview
 One of the more onerous requirements for calling ARM (Azure Resource Manager) is getting a JWT to authenticate yourself. This tool helps to generate the JWT in an easily consumable way for all of our environments. It can prompt for needed inputs or have all choices to passed as arguments, which makes it helpful as a shortcut for common usages. 

Usage
 Running the tool with no arguments gives the usage info:

C:\>ARMTokenApp.exe
Example usage: ArmTokenApp.exe -env Production -outputMethod TempFile [-subscriptionId 81eea2b6-df17-43a8-bb2b-a4a217d4de0d] [-userId me@live.com]
Example usage: ArmTokenApp.exe -inputMethod Interactive
Arguments:
    -env: Target environment can be Next, Current, Dogfood, Production
    [-outputMethod]: Token output method can be Print, TempFile, Clipboard
         Print: Prints token to standard out. This is the default.
         TempFile: Writes token to temp file to open in Notepad. Temp file is deleted immediately.
         Clipboard: Copies the token directly to the clipboard
    [-inputMethod]: Program input method can be Interactive, Arguments
         Arguments: Only looks at command line. This is the default.
         Interactive: Prompts for any missing input arguments
    [-subscriptionId]: Specify subscription to avoid choosing
    [-userId]: Specify user email to avoid typing or bypass AAD auto sign-in
 

 The output is the content of the Authorization header that must be set to call ARM. The output can be printed to standard out, copied to the clipboard, or opened in Notepad.

 The output will look like this: 

Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjVUa0d0S1JrZ2FpZXpFWTJFc0xDMmdPTGpBNCJ9.eyJhdWQiOiJodHRwczovL21hbmFn
ZW1lbnQuY29yZS53aW5kb3dzLm5ldC8iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLXBwZS5uZXQvYWIzNDIzNzYtN2ZiYS00NzE4LTk1MGQtZDAzYThm
ZDAyY2M1LyIsImlhdCI6MTQwNzU0NDE0NiwibmJmIjoxNDA3NTQ0MTQ2LCJleHAiOjE0MDc1NDgwNDYsInZlciI6IjEuMCIsInRpZCI6ImFiMzQyMzc2LTdm
YmEtNDcxOC05NTBkLWQwM2E4ZmQwMmNjNSIsImFtciI6WyJwd2QiXSwiYWx0c2VjaWQiOiIxOmxpdmUuY29tOjAwMDNCRkZEQzYyMzM4MjQiLCJpZHAiOiJs
aXZlLmNvbSIsIm9pZCI6IjAxOTE3N2Y5LTJjOTktNDc2My04OGY5LWUxZDNkYjYzOWUwNiIsInN1YiI6IkNEbWV0UEJQSl85OHJhUWZtLWtqN2xQVkdid0It
WDhzT2xiWnlUdTFlY1EiLCJlbWFpbCI6ImF1eHRtNzQ3QGxpdmUuY29tIiwiZ2l2ZW5fbmFtZSI6ImF1eHRtNzQ3QGxpdmUuY29tIiwiZmFtaWx5X25hbWUi
OiJhdXh0bTc0NyIsInVuaXF1ZV9uYW1lIjoibGl2ZS5jb20jYXV4dG03NDdAbGl2ZS5jb20iLCJhcHBpZCI6IjE5NTBhMjU4LTIyN2ItNGUzMS1hOWNmLTcx
NzQ5NTk0NWZjMiIsImFwcGlkYWNyIjoiMCIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsImFjciI6IjEifQ.Y2OO_lggkdwkALxudT-nHpXwGweOq-c-o
UFP2yPkET4mT08w-FWEoi61gW1CYeiviFsV-9HE2TLuz7Y2nnfmwkXTVA4B8EBvwlk3mv7kZ6SbL_FiO78rVDuTihMNgSIf6FhwgNr2eTxUDmgEXnSpavXDg
0o7w90HMYlslDOaCEaHU8KOc13Cj6A8-oS3cd07Fvok-DBTDlD92Uf9weeZPxHrQbrXvVo7bhx0i1Z4P2grRkwDA-RdN3Yic0_4CC7XttG4znJIlrsKROael
jf_8vkhiaIIw236Bq_PPLQbI86vac1f9oPtY78nrkve7ANgCuUCyYZ43_MdN7i3tKjoKw
 
 The output can be pasted into Postman or Fiddler as the "Authorization" header for calling ARM. Console app works too, just make sure the Authorization request header contains that token.

Tips
  •If you have a frequent combination of subscription/environment/user, create a shortcut for it with output set to "Clipboard". Then you'll just need to double-click the shortcut and possibly enter your password if the machine's credential cache has expired.

    ◦Create shortcut like this: \\scratch2\scratch\saban\ARMTokenApp\bin\ARMTokenApp.exe -env Production -outputMethod Clipboard -userId alias@microsoft.com -subscription asd-asd-adff-asdf-asdf

  •If desktop shortcuts aren't your style, create aliases in your enlistment! Alias something like "jwtNext" or "armtokennext" to prompt for test creds in NEXT and open in a new notepad

    ◦The alias value would look like this: "\\scratch2\scratch\saban\ARMTokenApp\bin\ARMTokenApp.exe -env Next -outputMethod TempFile"

  •Sometimes the AD sign-in prompt will try to be clever and log you in automatically. This can be painful to when you have multiple test accounts. ARMTokenTool can help!

    ◦Use the -userId argument (or provide a user email in the Interactive mode) to force the prompt for a particular user. It's also nice to avoid multiple login pages.

  •You may notice multiple empty windows pop up after loggin in. This is the tool walking through all tenants to get token. You can avoid this and the tenant chooser to make the tool faster!

    ◦User the -subscriptionId argument if you know which subscription you want to access. Then the JWT is obtained directly for that subscription's tenant without flashing pages or addition interaction.
