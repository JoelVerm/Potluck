# Test plan

All tests should run on every commit to any branch and every pull request to main. The different kinds of tests that are ran are described below.

## Code quality

For the frontend, code quality is ensured by following ESLint. It mainly checks syntax errors, but also acts as a small style guide preventing problems such as unused variables.

For the backend, Qodana is used, a utility by JetBrains. This program runs extensive checks about null reference exceptions, unused or redundant code, memory issues and encapsulation problems.

## Unit testing

The logic of the backend is tested using MSTest. The goal is to test every meaningful method, so excluding methods that just get data out of the database. Most of the time, parameterized tests are used to make testing many inputs a lot easier.

## End-to-end testing / integration testing

To test if the entire application still works, after all other tests have completed successfully docker containers are built and using a docker compose the entire app is brought online. Then Playwright runs end-to-end tests on the whole application.

Examples of tests are:

- Setting the user's dinner status to take two friends, and then checking if 3 people are actually registered for dinner.
- Changing the user's home status and testing if it updates in the list.
- Editing the name of the user's house and checking if it is visible after a reload.
- Changing the user's diet preferences and testing if it shows up on the dinner list.

## Deploying

After all tests have run and succeeded, the frontend and backend dockerfile are built and the containers uploaded to docker hub. Then, the running application should pull the new containers and run those.
