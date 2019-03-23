# PwnMe
![Coverage](https://img.shields.io/badge/coverage-78%25-yellowgreen.svg)
![Open Issues](https://img.shields.io/github/issues-raw/Mehigh17/PwnMe.svg)
![License](https://img.shields.io/github/license/Mehigh17/PwnMe.svg)

PwnMe is a small library wrapping Troy Hunt's [HIBP API](https://haveibeenpwned.com/API/v2). Is implements most of the vital functions.

## Usage

I recommend taking a quick glance on Troy's [website](https://haveibeenpwned.com/API/v2) if you're not familiar with what this API does.

*NB: The term 'account' signifies a valid email address or an username.*

### Wrapper

#### Get an account pastes
```cs
var client = new PwnMeClient();

// Gets the paste list for 'some@accou.nt', which could include informations about the source of the paste, or the date, etc.
var pastes = await client.GetAccountPastes("some@accou.nt")
```

#### Get a password occurence count
```cs
var client = new PwnMeClient();

// Number signifying how many times this password has been breached.
var count = await client.GetOccurences("Password123");

// Example of use
if(count > 5)
{
    /* ...
     * Invalidate user action
     */
    InformUser("This password has been breached too many times, use another one!");
}
```
#### Verify if a password has been pwned
```cs
var client = new PwnMeClient();
var isPwdBreached = await client.IsPwned("Password123");

// Example of use
if(isPwdBreached)
{
    /* ...
     * Invalidate user action
     */
    InformUser("This password has been breached, please use another one.");
}
```

#### Get account breaches
```cs
var client = new PwnMeClient();

// Gets all the breaches for a given account on 'some.domain', including all the additional details and excluding the unverified breaches.
var breaches = await client.GetAccountBreaches("test@accou.nt", truncated: false, domain: "some.domain", includeUnverified: false);
```

#### Get breaches for a domain
```cs
var client = new PwnMeClient();

// Gets all the breaches for a specified domain
var breaches = await client.GetBreaches("some.domain");
```