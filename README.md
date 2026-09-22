# RaceDay - Event Management System

## System Description
RaceDay is a full-stack web-based event management system designed specifically for the South African road running, walking, and cycling community. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, track their personal performance history, and prepare for race day using live weather and route information.

## Roles
The system supports two distinct user roles:

### Organiser
- Can create, edit, and delete events
- Can manage event categories
- Can capture participant results
- Can view all event enrolments

### Participant
- Can create an account
- Can browse events
- Can enter an event by selecting a category
- Can view their own enrolments
- Can track their personal results

## CI/CD Build Status
![CI/CD Green Build](screenshots/ci-cd-green-build.png)

## Video Presentation
[Watch the Part 1 Video Presentation] 

## Setup Instructions
1. Clone the repository: `git clone https://github.com/Abulele-26/RaceDay-PoE.git`
2. Navigate to the `/docs` folder to view the ERD, API endpoint plan, and SQL script.
3. Run the SQL script in SSMS on a clean SQL Server instance.

## Folder Structure
- `/docs` - Planning documents (ERD, API Endpoint Plan, SQL Script)
- `/.github/workflows` - CI/CD workflow
- `/screenshots` - CI/CD green build screenshot