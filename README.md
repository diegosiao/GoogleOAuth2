# GoogleOAuth2
Google APIs Access Token generator

## What it does?

This Windows CLI utility implements the Google APIs OAuth2 authorization using Google Libraries and generates an output file with the Access Token information.

Json and "Key=Value" output formats supported.

## How to use?

You will need to configure a service account and generate a '*.p12' certificate with appropriate security settings on [Google Developers Console](https://console.developers.google.com). Make sure to specify the scope under -s option using a comma separated pattern **without spaces** to specify more than one.

For basic usage, execute:

    GoogleOAuth2 -u service-account@your-project.iam.gserviceaccount.com -c certificate.p12 -s "https://www.googleapis.com/auth/spreadsheets"

For help with another options, execute:

    GoogleOAuth2 --help

JSON output example:

    {
        "access_token": "ya29.c.ElolBh1E0-9GSkssqauNE4ZmmYWhje3Pxkx8CF8V0AU6ddG51fplaJuB7NHJkRgO_Hch16WBSMR7BD7N14B-397Q8iAKlCSp7-65CixQC8QgxdZE_QHMEYkXOAs",
        "token_type": "Bearer",
        "expires_in": 3600,
        "refresh_token": null,
        "scope": null,
        "id_token": null,
        "Issued": "2018-09-26T23:19:02.5880612-03:00",
        "IssuedUtc": "2018-09-27T02:19:02.5880612Z"
    }

## Download

#### [GoogleOAuth2.exe](https://github.com/diegosiao/GoogleOAuth2/releases/download/1.0.3/GoogleOAuth2.exe)
