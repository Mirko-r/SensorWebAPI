import "./App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Visual from "./components/Visual";
import Add from "./components/Add";
import Edit from "./components/Edit";
import Details from "./components/Details";

function App() {
  return (
    <Router>
      <Routes>
        <Route exact path="/" element={<Visual />} />
        <Route path="/add" element={<Add />} />
        <Route path="/edit" element={<Edit />} />
        <Route path="/details" element={<Details />} />
      </Routes>
    </Router>
  );
}

export default App;
