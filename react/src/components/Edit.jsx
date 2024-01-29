import { Link } from "react-router-dom";
export default function Edit() {
  return (
    <>
      <h1>edit</h1>

      <form>
        <input type="text" />
        <br />
        <Link className="btn btn-secondary mt-3" to="/">
          CONFIRM
        </Link>{" "}
      </form>
    </>
  );
}
