import { Link } from "react-router-dom";

export default function Details() {
  return (
    <>
      <h1>detail</h1>

      <p>qua metti i dettagli del prodotto</p>

      <Link className="btn btn-secondary mt-3" to="/">
        BACK
      </Link>
    </>
  );
}
