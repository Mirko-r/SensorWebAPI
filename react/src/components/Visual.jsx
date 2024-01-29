import { useState, useEffect } from "react";
import axios from "axios";
import Swal from "sweetalert2";


export default function Visual() {  
        const [visual, setVisual] = useState([]);
        const [locationFilter, setLocationFilter] = useState("");
        useEffect(() => {
          fetchVisual();
        }, []);
      
        const fetchVisual = () => {
          axios.get("https://192.168.4.182:7014/api/Productions")
            .then(function (response) {
              // fulfilled
              console.log(response.data);
              setVisual(response.data);
            })
            .catch(function (error) {
              // rejected
              console.log(error);
            });
        }
        function handleDelete(id) {
            Swal.fire({
              title: "Are you sure?",
              text: "You cannnot undo this one",
              icon: "warning",
              showCancelButton: true,
              confirmButtonText: "Yes, delete it",
            }).then((result) => {
              if (result.isConfirmed) {
                axios
                  .delete("https://192.168.4.182:7014/api/Productions/" + id)
                  .then(function (response) {
                    Swal.fire({
                      icon: "success",
                      title: "production deleted successfully",
                      showConfirmButton: false,
                      timer: 1500,
                    });
                    fetchVisual();
                  })
                  .catch(function (error) {
                    Swal.fire({
                      icon: "error",
                      title: "An error occurred!",
                      showConfirmButton: false,
                      timer: 1500,
                    });
                    console.log(error);
                  });
              }
            });
          }

          const handleLocationFilterChange = (e) => {
            axios.get("https://192.168.4.182:7014/api/Productions/location/" + e.target.value)
            .then(function (response) {
              // fulfilled
              console.log(response.data);
              setVisual(response.data);
            })
            .catch(function (error) {
              // rejected
              console.log(error);
            });
          };
        
          

  return (
    <>
      <div className="container-sm">
        <div>
          <h1>table test</h1>
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

        <table className="table table">
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
              <th className="col">action</th>

            </tr>
          </thead>
          <tbody>
            {visual.map((visual, key) => {
              return (
                <tr key={key}>
                  <td>{visual.productionId}</td>
                  <td>{visual.machineId}</td>
                  <td>{visual.year}</td>
                  <td>{visual.machineLocation}</td>
                  <td>{visual.month}</td>
                  <td>{visual.day}</td>
                  <td>{visual.hourOfDay}</td>
                  <td>{visual.make}</td>

                  <td>

                
                <button
                      className="btn my-1 me-2 float-end btn-danger"
                      onClick={() => handleDelete(visual.productionId)}
                    > delete </button>
            
              </td>
            </tr>
            );
            })}
          </tbody>
        </table>

      </div>
    </>
  );
}
