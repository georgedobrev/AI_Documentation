import { useFormik } from 'formik';
import * as Yup from 'yup';
import './LoginPageStyles.css';
import logo from '../assets/DocuAuroraLogo_prev_ui.png'; 

const validationSchema = Yup.object({
    username: Yup.string()
        .min(2, 'Too Short!')
        .max(20, 'Too Long!')
        .required('Required'),
    password: Yup.string()
        .min(8, 'Password is too short - should be 8 chars minimum.')
        .matches(/[A-Z]/, 'Password must contain at least one uppercase letter.')
        .matches(/\d/, 'Password must contain at least one digit.')
        .matches(/[+\-*&^]/, 'Password must contain at least one special character (+, -, *, &, ^).')
        .required('Required'),
});

function LoginPage() {
    const formik = useFormik({
        initialValues: {
            username: '',
            password: '',
        },
        validationSchema: validationSchema,
        onSubmit: (values) => {
            console.log(values);
        },
    });

    return (
        <div className='App'>
            <div className='login-page'>
                <form onSubmit={formik.handleSubmit} className='login-form'>
                    <img src={logo} alt="Logo" className="logo" />
                    <h2>Login</h2>
                    <input
                        type='text'
                        id='username'
                        name='username'
                        placeholder='Username'
                        value={formik.values.username}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        required
                        className={formik.touched.username && formik.errors.username ? "error-input" : "normal-input"}
                    />
                    {formik.touched.username && formik.errors.username ? (
                        <div className="error">{formik.errors.username}</div>
                    ) : null}
                    <input
                        type='password'
                        id='password'
                        name='password'
                        placeholder='Password'
                        value={formik.values.password}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        required
                        className={formik.touched.password && formik.errors.password ? "error-input" : "normal-input"}
                    />
                    {formik.touched.password && formik.errors.password ? (
                        <div className="error">{formik.errors.password}</div>
                    ) : null}
                    <button type='submit'>Login</button>
                    <div className='google-login'>
                        <button onClick={(event) => console.log("Google Login Clicked!")}>
                            <img src='https://upload.wikimedia.org/wikipedia/commons/5/53/Google_%22G%22_Logo.svg' alt='Google logo' width='20' />
                            <span>Sign in with Google</span>
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default LoginPage;