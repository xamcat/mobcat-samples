# Introduction 
This repository contains sample use cases for an in memory repository implementation

## Samples
1. The first sample demonstrates a simple implementation for in memory repository items
2. The second sample shows how to fetch relational items with queries using the InMemorySampleRepositoryContext. The items are:
  - SampleUserModel
  - SampleOrderModel. Each order is tied to a User through the UserId Property.
  
## Diagram
Here is a diagram of how the sample implements the MobCAT repository pattern:


## Reasoning/Rationale
The pattern exposes high level methods through the InMemorySampleRepositoryContext which impelments IOfflineSampleRepositoryContext. To implement a different repository type (SQLite, EFCore, Akavache, etc), all it would need to do is implement IOfflineSampleRepositoryContext and be registered in App.xaml.cs.
The reasoning behind the RepositoryContext is to avoid having to cast objects directly from the Business Logic type to the underlying implementation type. All the translation is done at the repository level, providing better encapsulation and separation of reponsibilities. This also allows the business logic models to be totally independent of the repository models.

From the perspective of the app, this single point of entry to the repository provides a simple way to mock the repository for different environments, demos, QA testing, and automated testing.

From the perspective of the library, this reduces complexity for the consumer. All the consumer needs to do is the RepositoryContext interface directly and swap out the dependency service registration.

From the perspective of the consumer, this allows for more flexibility in implementation where the semantics and intricacies of storage mechanisms is up to each underlying storage technology. 
