import { useFormik } from "formik";
import * as Yup from "yup";
import "../Styles/ForgotPasswordStyles.css";
import logo from "../assets/DocuAuroraLogo_prev_ui.png";
import { sendResetPasswordLink } from "../Service/api";
import "../Styles/global.css";

const validationSchema = Yup.object({
  email: Yup.string()
    .email("Invalid email address.")
    .required("Email is required."),
});

function ForgotPasswordPage() {
  const formik = useFormik({
    initialValues: {
      email: "",
    },
    validationSchema: validationSchema,
    onSubmit: async (values) => {
      try {
        const user = await sendResetPasswordLink(values);
        //todo
      } catch (error) {
        console.error(error);
      }
    },
  });

  return (
    <div className="App">
      <div className="login-page">
        <form onSubmit={formik.handleSubmit} className="login-form">
          <img src={logo} alt="Logo" className="logo" />
          <h2>Forgot Password</h2>
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
          <button type="submit">Send Reset Link</button>
        </form>
      </div>
    </div>
  );
}

export default ForgotPasswordPage;
