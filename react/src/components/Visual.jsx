import { useState, useEffect } from "react";
import axios from "axios";
import Swal from "sweetalert2";

export default function Visual() {
  const [originalVisual, setOriginalVisual] = useState([]);
  const [filteredVisual, setFilteredVisual] = useState([]);
  const [locationFilter, setLocationFilter] = useState("");

  useEffect(() => {
    fetchVisual();
  }, []);

  const fetchVisual = () => {
    axios.get("https://192.168.4.182:7014/api/Productions")
      .then(function (response) {
        setOriginalVisual(response.data);
        setFilteredVisual(response.data);
      })
      .catch(function (error) {
        console.log(error);
      });
  };

  const handleDelete = (id) => {
    // ... (resto del codice rimane invariato)
  };

  const handleLocationFilterChange = (e) => {
    setLocationFilter(e.target.value);
    filterData(e.target.value);
  };

  const filterData = (filter) => {
    const filteredData = originalVisual.filter((item) =>
      item.machineLocation.toLowerCase().includes(filter.toLowerCase())
    );
    setFilteredVisual(filteredData);
  };

  return (
    <>
      <div className="container-sm">
        <div>
          <h1>PRODUCTIONS</h1>
        </div>

        <div className="mb-3">
          <label htmlFor="locationFilter" className="form-label">Location Filter:</label>
          <input
            type="text"
            className="form-control"
            id="locationFilter"
            value={locationFilter}
            onChange={(e) => handleLocationFilterChange(e)}
          />
        </div>

        <table className="table table-bordered">
          <thead>
            <tr>
              <th className="col">Productions</th>
              <th className="col">Sensor</th>
              <th className="col">year</th>
              <th className="col">location</th>
              <th className="col">month</th>
              <th className="col">day</th>
              <th className="col">hourofday</th>
              <th className="col">make</th>

            </tr>
          </thead>
          <tbody>
            {filteredVisual.map((currentVisual, key) => (
              <tr key={key}>
                <td>{currentVisual.productionId}</td>
                  <td>{currentVisual.machineId}</td>
                  <td>{currentVisual.year}</td>
                  <td>{currentVisual.machineLocation}</td>
                  <td>{currentVisual.month}</td>
                  <td>{currentVisual.day}</td>
                  <td>{currentVisual.hourOfDay}</td>
                  <td>{currentVisual.make}</td>

              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
