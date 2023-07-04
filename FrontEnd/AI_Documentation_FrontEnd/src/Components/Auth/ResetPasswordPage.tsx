import { useFormik } from "formik";
import * as Yup from "yup";
import { useNavigate } from "react-router-dom";
import logo from "../../assets/DocuAuroraLogo_prev_ui.png";
import "../../Styles/ResetPasswordPageStyles.css";
import { resetPassword } from "../../Service/api";
 
const validationSchema = Yup.object({
  newPassword: Yup.string().required("New password is required."),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref('newPassword'),], 'Passwords must match')
    .required('Confirm password is required'),
});
 
function ResetPasswordPage() {
  const navigate = useNavigate();
 
  const formik = useFormik({
    initialValues: {
      newPassword: "",
      confirmPassword: "",
    },
    validationSchema: validationSchema,
    onSubmit: async (values) => {
      try {
        await resetPassword(values.newPassword);
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
            type="password"
            id="newPassword"
            name="newPassword"
            placeholder="New password"
            value={formik.values.newPassword}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            required
            className={
              formik.touched.newPassword && formik.errors.newPassword
                ? "error-input"
                : "normal-input"
            }
          />
          {formik.touched.newPassword && formik.errors.newPassword ? (
            <div className="error">{formik.errors.newPassword}</div>
          ) : null}
          <input
            type="password"
            id="confirmPassword"
            name="confirmPassword"
            placeholder="Confirm password"
            value={formik.values.confirmPassword}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            required
            className={
              formik.touched.confirmPassword && formik.errors.confirmPassword
                ? "error-input"
                : "normal-input"
            }
          />
          {formik.touched.confirmPassword && formik.errors.confirmPassword ? (
            <div className="error">{formik.errors.confirmPassword}</div>
          ) : null}
          <button type="submit">Confirm</button>
        </form>
      </div>
    </div>
  );
}
 
export default ResetPasswordPage;
