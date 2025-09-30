# Hedgehog Trade Hub

A hub for users to trade items. 

## User Stories

https://revaturetech-my.sharepoint.com/:w:/g/personal/tevin708_revature_net/EU0gblsMjxpAr_tGBHMHMgEBmofDabaSnAn1ZbMcXj9wng?e=ikqzFu

## Endpoints

### Users

- [ ] GET /users - get all users (admin only?)
- [ ] POST /users - create new user
- [ ] POST /login - log in
- [ ] GET /users/me - get currently logged-in user(?)
- [ ] PATCH /users/me - modify currently logged-in user(?)
- [ ] GET /users/{UserId} - get specific user
- [ ] PATCH /users/{UserId} - modify specific user
- [ ] DELETE /users/{UserId} - delete user

### Items

- [ ] GET /items - get all items (in active trades) (parameters as search terms)
- [ ] POST /items - create new item
- [ ] GET /items/{ItemId} - get specific item
- [ ] PATCH /items/{ItemId} - modify item
- [ ] DELETE /items/{ItemId} - delete item

### Trades

- [ ] GET /trades - get all trades (admin only?)
- [ ] POST /trades - create new trade
- [ ] GET /trades/{TradeId} - get info on specific trade
- [ ] DELETE /trades/{TradeId} - close trade
- [ ] POST /trades/{TradeId}/items - add items to trade
- [ ] DELETE /trades/{TradeId}/items/{ItemId} - remove item from trade

### Offers

- [ ] GET /trades/{TradeId}/offers - get all offers
- [ ] POST /trades/{TradeId}/offers - create new offer
- [ ] GET /trades/{TradeId}/offers/{OfferId} - get info on specific offer (and all attached items?)
- [ ] POST /trades/{TradeId}/offers/{OfferId} - add items to offer
- [ ] DELETE /trades/{TradeId}/offers/{OfferId}/items/{ItemId} - delete specific item from offer

## Requirements

### Application Architecture

- [X] Your code must be pushed to a project git repo on the cohort organization
- [ ] Your application must build and run
- [ ] Your application components must be loosely coupled, and exemplify a Service Oriented Architecture

### SQL Database

- [ ] Your database must use MS SQL Server
- [ ] Your database must run inside of a docker container
- [X] Your database should be in 3rd normal form
- [X] Your database should include at least one many-to-many relationship
- [ ] Your database should be set up through an Entity Framework "Code-First" migration

### API

- [ ] Your API must be written in C# for .NET
- [ ] Your API must use the ASP.NET framework
- [ ] Your API must exemplify the principles of REST
- [ ] Your API must implement the Repository pattern for data persistance (multiple namespaces, interface)
- [ ] Your API must implement SQL Server and Entity Framework Core to provide data persistance
- [ ] Your API should fulfill all common CRUD functions
- [ ] Your API should include proper exception handling
- [ ] Your API should include logging with Serilog
- [ ] Your API should include validation for any data being handled
- [ ] Your API should include at least 50% unit test coverage

### Frontend

- [ ] Your frontend must be written in "vanilla JavaScript", with HTML5, CSS3, and JavaScript
- [ ] Your frontend must interact with your API for data persistence
- [ ] Your frontend may use the React library.
    - If you choose to use React, your frontend must include at least three components, with at least one nested component. (At least one parent-child relationship, and at least one other unrelated component).
        - [ ] Your components should include at least one component that implements `useState`.
        - [ ] Your components should include at least one component with props.
        - [ ] Your components should include at least one component that implements the `Context` hook.
- [ ] Your frontend __may__ be built on the Node.js or Next.js frameworks.

### Non-Functional Requiremnts

- [ ] Your project team should select a Team Lead to organize the development team efforts
- [ ] Your project should include a Project Description document which details:
    - [ ] a description of the project
    - [ ] at least five user stories for your applicaton
    - [ ] a wireframe diagram of the UI
    - [ ] an ERD (Entity Relationship Diagram) of the Database
    - [ ] unit test coverage reporting
    - [ ] API endpoint documentation
- [ ] Your project team should conduct a daily stand-up meeting
    - [ ] meeting notes should be kept to record development progress

### Project Presentation

- Project presentation will be done on Friday 10/10 afternoon.
- You should demo the application functionality by completing the planned user stories
- Your presentation should be about 10 minutes long, no longer than 15 minutes (at 20 min, you will be cut off so that there is time for everyone to present)
