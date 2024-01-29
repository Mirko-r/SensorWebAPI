import { Link } from "react-router-dom";

export default function Visual() {  
        const [projectList, setProjectList] = useState([]);
      
        useEffect(() => {
          fetchProjectList();
        }, []);
      
        const fetchProjectList = () => {
          axios.get("https://localhost:7138/api/projects")
            .then(function (response) {
              // fulfilled
              console.log(response.data);
              setProjectList(response.data);
            })
            .catch(function (error) {
              // rejected
              console.log(error);
            });
        }
  return (
    <>
      <div className="container-sm">
        <div>
          <h1>table test</h1>
        </div>

        <table className="table table-bordered">
          <thead>
            <tr>
              <th className="col">Productions</th>
              <th className="col">Car</th>
              <th className="col">Location</th>
              <th className="col">Action</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>prod 1</td>
              <td>car 1</td>
              <td>turin</td>
              <td>
                <Link className="btn btn-secondary mt-3" to="/edit">
                  EDIT
                </Link>
                <Link className="btn btn-secondary mt-3" to="/details">
                  DETAILS
                </Link>
              </td>
            </tr>
          </tbody>
        </table>
        <Link className="btn btn-secondary mt-3" to="/add">
          ADD
        </Link>
      </div>
    </>
  );
}
