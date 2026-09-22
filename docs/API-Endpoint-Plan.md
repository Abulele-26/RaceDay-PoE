# RaceDay API Endpoint Plan

This document outlines all the API endpoints for the RaceDay system. It covers the required functionality for both Organisers and Participants.

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
| :--- | :--- | :--- | :--- | :--- | :--- |
| POST | /api/auth/register | Registers a new user account (Organiser or Participant) | None (public) | { "email": "string", "password": "string", "fullName": "string", "role": "Participant" } | 201 Created - user registered<br>400 Bad Request - validation failed<br>409 Conflict - email already exists |
| POST | /api/auth/login | Authenticates a user and returns a JWT token | None (public) | { "email": "string", "password": "string" } | 200 OK - returns JWT token<br>400 Bad Request - missing fields<br>401 Unauthorized - invalid credentials |
| GET | /api/users/me | Retrieves the profile of the currently logged-in user | Any (logged in) | None | 200 OK - user profile object<br>401 Unauthorized - not logged in |
| PUT | /api/users/me | Updates the profile of the currently logged-in user | Any (logged in) | { "fullName": "string", "email": "string" } | 200 OK - profile updated<br>400 Bad Request - validation failed<br>401 Unauthorized - not logged in |
| GET | /api/events | Retrieves a list of all upcoming events | None (public) | None | 200 OK - array of event objects |
| GET | /api/events/{id} | Retrieves details of a specific event | None (public) | None | 200 OK - event object<br>404 Not Found - event does not exist |
| POST | /api/events | Creates a new event | Organiser | { "name": "string", "description": "string", "eventDate": "datetime", "location": "string" } | 201 Created - event created<br>400 Bad Request - validation failed<br>401 Unauthorized - not logged in<br>403 Forbidden - not an Organiser |
| PUT | /api/events/{id} | Updates an existing event | Organiser | { "name": "string", "description": "string", "eventDate": "datetime", "location": "string" } | 200 OK - event updated<br>400 Bad Request - validation failed<br>404 Not Found - event does not exist<br>403 Forbidden - not the event owner |
| DELETE | /api/events/{id} | Deletes an event | Organiser | None | 204 No Content - event deleted<br>404 Not Found - event does not exist<br>403 Forbidden - not the event owner |
| GET | /api/events/{eventId}/categories | Retrieves all categories for a specific event | None (public) | None | 200 OK - array of category objects<br>404 Not Found - event does not exist |
| POST | /api/events/{eventId}/categories | Creates a new category for an event | Organiser | { "name": "string", "distance": "decimal", "entryFee": "decimal" } | 201 Created - category created<br>400 Bad Request - validation failed<br>403 Forbidden - not the event owner |
| PUT | /api/categories/{id} | Updates a specific category | Organiser | { "name": "string", "distance": "decimal", "entryFee": "decimal" } | 200 OK - category updated<br>400 Bad Request - validation failed<br>404 Not Found - category does not exist<br>403 Forbidden - not the event owner |
| DELETE | /api/categories/{id} | Deletes a specific category | Organiser | None | 204 No Content - category deleted<br>404 Not Found - category does not exist<br>403 Forbidden - not the event owner |
| POST | /api/events/{eventId}/enroll | Enrolls the logged-in participant into a specific event category | Participant | { "categoryId": "int" } | 201 Created - enrollment successful<br>400 Bad Request - invalid category<br>401 Unauthorized - not logged in<br>409 Conflict - already enrolled |
| GET | /api/enrollments/me | Retrieves all enrollments for the currently logged-in participant | Participant | None | 200 OK - array of enrollment objects<br>401 Unauthorized - not logged in |
| DELETE | /api/enrollments/{id} | Cancels an enrollment | Participant | None | 204 No Content - enrollment cancelled<br>404 Not Found - enrollment does not exist<br>403 Forbidden - not the participant who enrolled |
| GET | /api/results/me | Retrieves all personal results for the logged-in participant | Participant | None | 200 OK - array of result objects<br>401 Unauthorized - not logged in |
| POST | /api/events/{eventId}/results | Captures participant results for a specific event | Organiser | { "enrollmentId": "int", "finishTime": "time", "position": "int" } | 201 Created - result added<br>400 Bad Request - validation failed<br>403 Forbidden - not the event owner |
| GET | /api/events/{eventId}/results | Retrieves all results for a specific event | None (public) | None | 200 OK - array of result objects<br>404 Not Found - event does not exist |