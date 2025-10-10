import { index, route } from "@react-router/dev/routes";

export default [
  index("./routes/home.jsx"),
  route("search", "./routes/search.jsx"),
  route("login", "./routes/Auth/LoginPage.jsx"),
  route("register", "./routes/Auth/RegisterPage.jsx"),
];
