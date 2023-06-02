import { useFormik } from 'formik';
import * as Yup from 'yup';
import '../Styles/RegisterPageStyles.css';
import logo from '../assets/DocuAuroraLogo_prev_ui.png';
import { registerUser } from '../Services/api'; // adjust path according to your project structure

const validationSchema = Yup.object({
    username: Yup.string()
        .min(2, 'Username is too short - should be 2 chars minimum.')
        .max(20, 'Username is too long - should be 20 chars maximum.')
        .required('Username is required.'),
    email: Yup.string()
        .email('Invalid email address')
        .required('Email is required.'),
    password: Yup.string()
        .min(8, 'Password is too short - should be 8 chars minimum.')
        .matches(/[A-Z]/, 'Password must contain at least one uppercase letter.')
        .matches(/\d/, 'Password must contain at least one digit.')
        .matches(/[+\-*&^]/, 'Password must contain at least one special character (+, -, *, &, ^).')
        .required('Password is required.'),
});

function RegisterPage() {
    const formik = useFormik({
        initialValues: {
            username: '',
            email: '',
            password: '',
        },
        validationSchema: validationSchema,
        onSubmit: async (values) => {
            try {
                const user = await registerUser(values);
                console.log(user);
                // Here, you can set user data into a state management library (like Redux or Zustand),
                // local storage or pass it to another component via props or context API
            } catch (error) {
                console.error(error);
                // Handle error as per your needs
            }
        },
    });

    return (
        <div className='App'>
            <div className='register-page'>
                <form onSubmit={formik.handleSubmit} className='register-form'>
                    <img src={logo} alt='Logo' className='logo'/>
                    <h2>Register</h2>
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
                        type='email'
                        id='email'
                        name='email'
                        placeholder='Email'
                        value={formik.values.email}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        required
                        className={formik.touched.email && formik.errors.email ? "error-input" : "normal-input"}
                    />
                    {formik.touched.email && formik.errors.email ? (
                        <div className="error">{formik.errors.email}</div>
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
                    <button type='submit'>Register</button>
                    <div className='google-login'>
                        <button onClick={(event) => console.log("Google Register Clicked!")}>
                            <img src='https://upload.wikimedia.org/wikipedia/commons/5/53/Google_%22G%22_Logo.svg' alt='Google logo' width='20' />
                            <span>Sign up with Google</span>
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default RegisterPage;
