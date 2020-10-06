# CatApi

Welcome to CatApi.

This is a very basic example of ASP.NET Core WebAPI used in a several courses
on Lublin University of Technology as a fake data source for mobile or web
apps.

CatApi is bulit over a `cats.json` data file, a <http://thecatapi.com/>-based
database, with some info about some cats. Data source is loaded in a static
constructor and is of course completely not persisting any changes, but **it is
all intended**.

```csharp
{
    "Id": "3m3",
    "Url": "http://25.media.tumblr.com/tumblr_m32hcqQtWW1qjc1a7o1_1280.jpg",
    "SourceUrl": "http://thecatapi.com/?id=3m3",
    "Name": "Hampton",
    "Loves": 0,
    "Hates": 0
  },
```

The data model has most importantly unique Id (alphanumeric), Name and Url
(image) of a particular cat, as well as number of Loves and Hates. SourceUrl is
used only as a homage to original data source.

The allowed operations are:

* GET `api/cats` - to get all of cats,
* GET `api/cat/<id>` - to get a single cat of a given id,
* PUT `api/love/<id>` - to increase number of loves of a cat with a given id,
* PUT `api/hate/<id>` - to increase number of hates of a cat with a given id.

Both "love" and "hate" operations require authorization, using HTTP Basic
Authentication method, where username must start with "user" and password must
end with "word".

Contributions to this project are licensed under a CC0 license. This project is
strictly educational, experimental and meant to be very basic.
