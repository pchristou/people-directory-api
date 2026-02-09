# People Directory API
A web API providing details for users contained within the directory. The UI can be found [here](https://github.com/pchristou/people-directory).

![People Directory Homepage](directory-api.png)

# Flow

The code is structured 
Controllers → Repository → DB

# Controllers

Our entry points into the endpoints

# Repository

Connect our code to our backend DB

# DB

The `users.json` file. Of course, in a true production setting, we'd use an actual DB (SQL or NoSQL).

Note that users can write to the JSON file. When in development and restarting the API, we repopulate `users.json` with `users.seed.json` to provide the same starting state as before. Useful for demos.