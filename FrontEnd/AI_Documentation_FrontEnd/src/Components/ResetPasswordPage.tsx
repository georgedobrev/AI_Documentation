import { useFormik } from "formik";
import * as Yup from "yup";
import { useNavigate } from "react-router-dom";
import logo from "../assets/DocuAuroraLogo_prev_ui.png";
import "../Styles/ResetPasswordPageStyles.css";
import { resetPassword } from "../Service/api";

const validationSchema = Yup.object({
  email: Yup.string()
    .email("Invalid email address")
    .required("Email is required."),
});

function ResetPasswordPage() {
  const navigate = useNavigate();

  const formik = useFormik({
    initialValues: {
      email: "",
    },
    validationSchema: validationSchema,
    onSubmit: async (values) => {
      try {
        const user = await resetPassword(values.email);
        navigate("/login");
      } catch (error) {
        console.error(error);
      }
    },
  });

  return (
    <div className="App">
      <div className="reset-password-page">
        <form onSubmit={formik.handleSubmit} className="reset-password-form">
          <img src={logo} alt="Logo" className="logo" />
          <h2>Reset Password</h2>
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
          <button type="submit">Send reset link</button>
        </form>
      </div>
    </div>
  );
}

export default ResetPasswordPage;
