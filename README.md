# Movies RESTful WebAPI

* [Overview](#overview)
* [Dependencies](#dependencies)
* [Documentation](#documentation)
* [Request & Response Examples](#request--response-examples)

## Overview

This document provides a summary of the Movies RESTful WebAPI, which was created as a proof-of-concept application to store data in an in-memory database and return it back over HTTP in JSON format.

*The database is seeded with a set of test data and is re-created on each launch of the application. A possible future enhancement would be to make the seeding optional via launch parameters.*

## Dependencies

The following frameworks and packages were used to implement the application:

* [ASP.NET Core 2.1.1](https://blogs.msdn.microsoft.com/dotnet/2018/06/22/net-core-2-1-june-update/)
* [Entity Framework Core 2.1.1 w/ SQLite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/)
* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
* [NUnit](https://www.nuget.org/packages/NUnit/) and [FluentAssertions](https://www.nuget.org/packages/FluentAssertions/) (unit tests)
* [Log4Net](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Log4Net.AspNetCore/) (debugging)
* [Swashbuckle](https://www.nuget.org/packages/Swashbuckle.AspNetCore/) (Swagger generator)

## Documentation

Full, interactive API documentation can be accessed from within the application via the route: **/swagger**

## Request & Response Examples

### API Methods

- [GET /api/movies](#get-apimovies)
- [GET /api/movies/{id}](#get-apimoviesid)
- [POST /api/movies](#post-apimovies)
- [PUT /api/movies/{id}](#put-apimoviesid)
- [DELETE /api/movies/{id}](#delete-apimoviesid)

### GET /api/movies

Gets all of the movies in the database

Possible response codes:

- 200: Returns the list of movies

Example: https://localhost:44366/api/movies

Response body:

 ```json  [
[  
   {  
      "$id":"73",
      "movieId":1,
      "title":"They Live",
      "description":"Science fiction thriller, directed by John Carpenter",
      "movieActors":[  
         {  
            "$id":"74",
            "movieId":1,
            "movie":{  
               "$ref":"73"
            },
            "actorId":1,
            "actor":{  
               "$id":"75",
               "actorId":1,
               "firstName":"'Rowdy' Roddy",
               "surname":"Piper",
               "movieActors":[  
                  {  
                     "$ref":"74"
                  }
               ]
            }
         },
         {  
            "$id":"76",
            "movieId":1,
            "movie":{  
               "$ref":"73"
            },
            "actorId":2,
            "actor":{  
               "$id":"77",
               "actorId":2,
               "firstName":"Keith",
               "surname":"David",
               "movieActors":[  
                  {  
                     "$ref":"76"
                  }
               ]
            }
         }
      ]
   },
   {  
      "$id":"78",
      "movieId":2,
      "title":"Taxi Driver",
      "description":"Martin Scorsese, neo-noir classic",
      "movieActors":[  
         {  
            "$id":"79",
            "movieId":2,
            "movie":{  
               "$ref":"78"
            },
            "actorId":3,
            "actor":{  
               "$id":"80",
               "actorId":3,
               "firstName":"Robert",
               "surname":"De Niro",
               "movieActors":[  
                  {  
                     "$ref":"79"
                  }
               ]
            }
         }
      ]
   }
]
```

### GET /api/movies/{id}

Get a specific movie by ID

Possible response codes:

- 200: Returns the movie requested
- 404: If a single movie with the requested ID cannot be found

Example: https://localhost:44366/api/movies/3

Response body:

```json
{  
   "$id":"29",
   "movieId":3,
   "title":"Cape Fear",
   "description":"Creepy boat scene, parodied by The Simpsons",
   "movieActors":[  
      {  
         "$id":"30",
         "movieId":3,
         "movie":{  
            "$ref":"29"
         },
         "actorId":3,
         "actor":{  
            "$id":"31",
            "actorId":3,
            "firstName":"Robert",
            "surname":"De Niro",
            "movieActors":[  
               {  
                  "$ref":"30"
               }
            ]
         }
      }
   ]
}
```


### POST /api/movies

Creates a new movie

Possible response codes:

- 201: The movie was created successfully
- 400: If a null model was passed in
- 404: If a referenced actor ID cannot be found

Sample request:

```json
POST /api/movies
{
   "Title":"Jurassic Park",
   "Description":"Dinosaurs",
   "MovieActors":[  
      {  
         "ActorId":5
      }
   ]
}
```

### PUT /api/movies/{id}

Update an existing movie's title or description

Possible response codes:

- 200: The movie was updated successfully
- 400: If a null model was passed in
- 404: If a single movie with the requested ID cannot be found

Sample request:

```json
PUT /api/movies/1
{  
   "Description":"This movie recently won an oscar"
}
```

### DELETE /api/movies/{id}

Delete a specified movie

Possible response codes:

- 200: The movie was updated successfully
- 404: If a single movie with the requested ID cannot be found
