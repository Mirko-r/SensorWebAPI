import axios from "axios";
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";


export default function Add() {

  
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const navigate = useNavigate();

  function handleSave() {
    axios
      .post("https://localhost:7138/api/projects", {
        name: name,
        description: description,
      })
      .then(function (response) {
        Swal.fire({
          icon: "success",
          title: "Projet saved successfully!",
          showConfirmButton: false,
          timer: 1500,
        })
        console.log(response.data);
        setName("");
        setDescription("");
        navigate("/");
      })
      .catch(function (error) {
        Swal.fire({
          icon: "error",
          title: "An error occurred!",
          showConfirmButton: false,
          timer: 1500,
        })
        console.log(error);
      });
  }

  return (
    <>
      <div className="container-sm pt-5">
      <h1 className="text-center text-primary">Create...</h1>
        <form className="w-75 mx-auto" action="">
          <label className="form-label text-start fw-bold">
            Inserisci nome
          </label>
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="form-control"
            type="text"
          />

          <label className="mt-2 form-label text-start fw-bold">
            Inserisci descrizione
          </label>
          <input
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="form-control"
            type="text"
          />
        </form>
        <div className="w-75 mx-auto">
        <Link className="btn btn-secondary mt-3" to="/">
        <i className="fa-solid fa-arrow-left"></i>
        </Link>
          <button className="mt-3 float-end btn btn-success" onClick={handleSave}>
            Post
          </button>
        </div>
      </div>
    </>
  );
}
