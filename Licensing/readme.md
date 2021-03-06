# Licensing library

## Usage:

    var options = new LicenseOptions { ... };
    var licenser = Licenser.Create(options);

Loads the registration information or creates (and saves) a new one.
Checks the status of the registration and sets the IsLicensed and IsTrial properties.

### Check for validity

    var isValid = licenser.IsLicensed;

This will return *true* if the application is licensed and *false* otherwise.

### Check if the trial is still available

    var isTrial = licenser.IsTrial;

This will return *true* if the trial is still available and *false* if the limits have been reached (the application should be closed in that case).

### Check whether the application should run

    var shouldRun = licenser.ShouldRun;

This will return *true* if the application should run (either the license is valid, or the trial hasn't reached its limits) and *false* otherwise.

### Showing registration information

    var details = licenser.LoadRegistration();

Reloads and re-checks the registration details; the application might then choose to display those details, eg a message showing how many days
are remaining. It is recommended that a text box be provided for the user to enter a license key; if that happens, the application can use the
SaveRegistration method (below).

Note: if the application has been copied from a different computer (for example), the registration details will be null. Treat as an expired trial.

### Create registration information (eg from the installer)

    var details = new LicenseRegistration { ... };
    licenser.SaveRegistration(details);

Saves the given registration details, optionally encrypted with a password. If CheckUrl is not empty, it will first try to register
the application at the given URL and only save to the file if everything went ok.

### Delete the registration information (e.g., on uninstall)

    licenser.DeleteRegistration();

### Sample code:

    var options = new LicenseOptions { ... };
    var licenser = Licenser.Create(options);

    // initial check
    if (!licenser.IsLicensed)
    {
        var details = licenser.LoadRegistration();
        if (details == null)
          Application.Terminate();
        
        // show registration / trial screen
    }

    // the customer might have just bought a license and entered the key, check again
    if (!licenser.IsLicensed && !licenser.IsTrial)
    {
        Application.Terminate();
    }

The double check can be simplified by using the ShouldRun property:

    var options = new LicenseOptions { ... };
    var licenser = Licenser.Create(options);

    // initial check
    if (!licenser.IsLicensed)
    {
        var details = licenser.LoadRegistration();
        if (details == null)
          Application.Terminate();
        
        // show registration / trial screen
    }

    // should I run the application?
    if (!licenser.ShouldRun)
    {
        Application.Terminate();
    }


## Options

Name        | Description
----------- | -----------
Password    | The encryption key; if not specified, the details will be unencrypted
Salt        | The encryption salt; if the key is specified, the salt should too. The salt should be at least 8 characters long
CheckUrl    | The link used to check the validity of the license (see below); if not specified, the license key is assumed valid if it exists and is a valid guid
SubmitUrl   | The link used to submit a new registration; if not specified it will default to the value of CheckUrl
Company     | The company name used for the registry key; defaults to the value in the entry assembly (the main application)
Product     | The product name used for the registry key; defaults to the value in the entry assembly (the main application)

### Checking the license

If CheckUrl is present, the licenser will first check that there is a key and that it 1) is a valid GUID and 2) has not expired. If that check fails,
the licenser treats the key as non-existent. If both conditions are met, the initialization makes a GET call to

https://{CheckUrl}?Key={key}&ProcessorId={id}

If the call fails or does not return a correct response, the key is considered invalid (the application is not registered).

A correct response will have the form

{key} {date}

With {date} formatted as yyyy-mm-dd. A permanent license will have a date of 9999-12-31.

The licenser will overwrite the expiration date in the registry with the given date.

### Registering

If SubmitUrl is present, the SaveRegistration method will (after making sure that the registration information is internally valid) send a POST request to

https://{SubmitUrl}

with Content-Type set to application/x-www-form-urlencoded and the LicenseRegistration details. The response from the server is not important
(as long as it doesn't throw an exception) because the library will immediately make a GET request to check the license.

Note: if SubmitUrl is not present but CheckUrl is, the library will use CheckUrl for the POST.

## Limits

The install can set up the following limits; at least one of them must be present:

Name   | Description
------ | -----------
Days   | The total number of days (-1 means unlimited)
Runs   | The total number of runs (-1 means unlimited)

If any of these conditions is met the trial expires - the validity check returns false.
