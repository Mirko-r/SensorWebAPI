import "./App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Visual from "./components/Visual";
import Login from "./components/Login";


function App() {
  return (
    <Router>
      <Routes>
        <Route exact path="/" element={<Login />} />
        <Route path="/visual" element={<Visual />} />
      </Routes>
    </Router>
  );
}

export default App;
