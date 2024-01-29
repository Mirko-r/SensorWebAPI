import { useState, useEffect } from "react";
import axios from "axios";

export default function Visual() {
  const [originalVisual, setOriginalVisual] = useState([]);
  const [filteredVisual, setFilteredVisual] = useState([]);
  const [locationFilter, setLocationFilter] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(20);

  useEffect(() => {
    if (locationFilter !== "") {
      filterData(locationFilter);
    } else {
      fetchVisual();
    }
  }, [currentPage, locationFilter]);

  useEffect(() => {
    const savedLocation = localStorage.getItem("lastLocation");
    if (savedLocation !== null) {
      setLocationFilter(savedLocation);
      // Non chiamare filterData(savedLocation) qui per evitare un doppio fetchVisual
    } else {
      // Se savedLocation è null o vuoto, imposta locationFilter a una stringa vuota
      setLocationFilter("");
    }
  }, []);

  useEffect(() => {
    localStorage.setItem("lastLocation", locationFilter);
  }, [locationFilter]);



  const fetchVisual = () => {
    axios.get("https://192.168.4.182:7014/api/Productions")
      .then(function (response) {
        setOriginalVisual(response.data);
        filterData(locationFilter);
      })
      .catch(function (error) {
        console.log(error);
      });
  };

  const handleLocationFilterChange = (e) => {
    const newLocation = e.target.value;
    setLocationFilter(newLocation);
    setCurrentPage(1);
    filterData(newLocation);
  };

  const filterData = (filter) => {
    const filteredData = originalVisual.filter((item) =>
      item.machineLocation.toLowerCase().includes(filter.toLowerCase())
    );
    setFilteredVisual(filteredData);
  };

  const totalPages = Math.ceil(filteredVisual.length / itemsPerPage);

  const handlePageChange = (newPage) => {
    if (newPage >= 1 && newPage <= totalPages) {
      setCurrentPage(newPage);
    }
  };

  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const slicedData = filteredVisual.slice(startIndex, endIndex);

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
              <th className="col">summake</th>
            </tr>
          </thead>
          <tbody>
            {slicedData.map((currentVisual, key) => (
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

        <div className="pagination">
          <button className="btn btn-secondary mt-3" onClick={() => handlePageChange(1)}>First</button>
          <button className="btn btn-secondary mt-3" onClick={() => handlePageChange(currentPage - 1)}>Previous</button>
          <span>Page {currentPage} of {totalPages}</span>
          <button className="btn btn-secondary mt-3" onClick={() => handlePageChange(currentPage + 1)}>Next</button>
          <button className="btn btn-secondary mt-3" onClick={() => handlePageChange(totalPages)}>Last</button>
        </div>
      </div>
    </>
  );
}
