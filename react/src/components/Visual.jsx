import { useState, useEffect } from "react";
import axios from "axios";
import "./index.css";


export default function Visual() {
  const [originalVisual, setOriginalVisual] = useState([]);
  const [filteredVisual, setFilteredVisual] = useState([]);
  const [locationFilter, setLocationFilter] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(20);

  useEffect(() => {
    fetchVisual();
  }, [currentPage, locationFilter]);

  useEffect(() => {
    fetchVisual();
  });
  const fetchVisual = () => {
    axios
      .get("https://192.168.4.182:7014/api/Productions")
      .then(function (response) {
        setOriginalVisual(response.data);
        filterData(locationFilter);
      })
      .catch(function (error) {
        console.log(error);
      });
  };

  const handleLocationFilterChange = (e) => {
    const updatedLocation = e.target.value.trim();
    setLocationFilter(updatedLocation);
    setCurrentPage(1);

    if (updatedLocation !== "") {
      filterData(updatedLocation);
    } else {
      fetchVisual();
    }
  };

  const filterData = (filter) => {
    const filteredData = originalVisual.filter((item) =>
      (item.machineLocation ? item.machineLocation.toLowerCase() : "").includes(
        filter.toLowerCase()
      )
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
          <h1 className="display-1">PRODUCTIONS</h1>
        </div>
        <hr className="border border-danger border-3 opacity-50"></hr>
        <div className="container mb-3">
          <label htmlFor="locationFilter" className="lead">
            Filter by location:
          </label>
          <input
            type="text"
            className="form-control"
            id="locationFilter"
            value={locationFilter}
            onChange={(e) => handleLocationFilterChange(e)}
          />
        </div>
        <hr className="border border-danger border-1 opacity-25"></hr>

        <table className="table table-hover item-center lead">
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

        <div className="pagination  justify-content-center mt-3">
          <button
            className="btn btn-secondary mt-3"
            onClick={() => handlePageChange(1)}
          >
            {"<<"}
          </button>
          <button
            className="btn btn-secondary mt-3 mx-2"
            onClick={() => handlePageChange(currentPage - 1)}
          >
            {"<"}
          </button>

          <div className="container mb-3">
            <label className="lead">
              Page {currentPage} of {totalPages}
            </label>
          </div>
          <button
            className="btn btn-secondary mt-3 mx-2"
            onClick={() => handlePageChange(currentPage + 1)}
          >
            {">"}
          </button>
          <button
            className="btn btn-secondary mt-3 "
            onClick={() => handlePageChange(totalPages)}
          >
            {">>"}
          </button>
        </div>
      </div>
      
    </>
  );
}
