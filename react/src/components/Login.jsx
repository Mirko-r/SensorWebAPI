import { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import Swal from 'sweetalert2';


export default function Login() {
  const [user, setUser] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post(
        'https://192.168.4.182:7014/api/Productions/login',
        {
          Username: user,
          Password: password
        }
      );

      const { token } = response.data;
      localStorage.setItem('token', token);
      console.log("Token salvato:", token);

      // Reindirizza manualmente il componente a /visual
      navigate("/visual");
    } catch (error) {
      console.error('Errore durante il login:', error);

      Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: 'Username o password non validi!',
      });
      // Gestire errori di login, ad esempio mostrando un messaggio di errore
    }
  };

  return (
    <div className="container-sm">
      <div>
        <h1 className="display-1">LOGIN</h1>
      </div>
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label htmlFor="user" className="form-label">
            User:
          </label>
          <input
            type="text"
            className="  form-control input-block m-2 m-2"
            id="user"
            value={user}
            onChange={(e) => setUser(e.target.value)}
          />
        </div>
        <div className="mb-3">
          <label htmlFor="password" className="form-label">
            Password:
          </label>
          <input
            type="password"
            className="  form-control input-block m-2 m-2"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        <button type="submit" className="btn btn-primary btn-login">
          SUBMIT
        </button>
      </form>
    </div>
  );
}
