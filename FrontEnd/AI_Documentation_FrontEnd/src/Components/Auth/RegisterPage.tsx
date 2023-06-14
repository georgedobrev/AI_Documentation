import { useFormik } from "formik";
import * as Yup from "yup";
import { useNavigate } from "react-router-dom";
import GoogleLogin from "@leecheuk/react-google-login";
import { GoogleResponse } from "../../Types/types";
import { registerUser, sendGoogleToken } from "../../Service/api";
import { GOOGLE_CLIENT_ID } from "../../config";
import logo from "../../assets/DocuAuroraLogo_prev_ui.png";
import "../../Styles/RegisterPageStyles.css";

const validationSchema = Yup.object({
  username: Yup.string()
    .min(2, "Username is too short - should be 2 chars minimum.")
    .max(20, "Username is too long - should be 20 chars maximum.")
    .required("Username is required."),
  email: Yup.string()
    .email("Invalid email address")
    .required("Email is required."),
  password: Yup.string()
    .min(8, "Password is too short - should be 8 chars minimum.")
    .matches(/[A-Z]/, "Password must contain at least one uppercase letter.")
    .matches(/\d/, "Password must contain at least one digit.")
    .matches(
      /[+\-*&^]/,
      "Password must contain at least one special character (+, -, *, &, ^)."
    )
    .required("Password is required."),
});

const responseGoogle = async (response: GoogleResponse) => {
  //todo

  if ("tokenId" in response) {
    try {
      const data = await sendGoogleToken(response.tokenId);
      //todo
    } catch (error) {
      console.error(error);
    }
  }
};

function RegisterPage() {
  const navigate = useNavigate();

  const formik = useFormik({
    initialValues: {
      username: "",
      email: "",
      password: "",
    },
    validationSchema: validationSchema,
    onSubmit: async (values) => {
      try {
        const user = await registerUser(values);
        //todo
        navigate("/login");
      } catch (error) {
        console.error(error);
      }
    },
  });

  const renderGoogleLoginButton = () => (
    <GoogleLogin
      clientId={GOOGLE_CLIENT_ID}
      onSuccess={responseGoogle}
      onFailure={responseGoogle}
      render={(renderProps) => (
        <button onClick={renderProps.onClick} disabled={renderProps.disabled}>
          <img
            src="https://upload.wikimedia.org/wikipedia/commons/5/53/Google_%22G%22_Logo.svg"
            alt="Google logo"
            width="20"
          />
          <span>Sign up with Google</span>
        </button>
      )}
    />
  );

  return (
    <div className="App">
      <div className="register-page">
        <form onSubmit={formik.handleSubmit} className="register-form">
          <img src={logo} alt="Logo" className="logo" />
          <h2>Register</h2>
          <input
            type="text"
            id="username"
            name="username"
            placeholder="Username"
            value={formik.values.username}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            required
            className={
              formik.touched.username && formik.errors.username
                ? "error-input"
                : "normal-input"
            }
          />
          {formik.touched.username && formik.errors.username ? (
            <div className="error">{formik.errors.username}</div>
          ) : null}
          <input
            type="email"
            id="email"
            name="email"
            placeholder="Email"
            value={formik.values.email}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            required
            className={
              formik.touched.email && formik.errors.email
                ? "error-input"
                : "normal-input"
            }
          />
          {formik.touched.email && formik.errors.email ? (
            <div className="error">{formik.errors.email}</div>
          ) : null}
          <input
            type="password"
            id="password"
            name="password"
            placeholder="Password"
            value={formik.values.password}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            required
            className={
              formik.touched.password && formik.errors.password
                ? "error-input"
                : "normal-input"
            }
          />
          {formik.touched.password && formik.errors.password ? (
            <div className="error">{formik.errors.password}</div>
          ) : null}
          <button type="submit">Register</button>
          <div className="google-login">{renderGoogleLoginButton()}</div>
        </form>
      </div>
    </div>
  );
}

export default RegisterPage;
