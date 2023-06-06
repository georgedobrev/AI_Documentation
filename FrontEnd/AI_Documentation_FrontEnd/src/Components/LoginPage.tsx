import { useFormik } from 'formik';
import * as Yup from 'yup';
import GoogleLogin from "@leecheuk/react-google-login";
import '../Styles/LoginPageStyles.css';
import logo from '../assets/DocuAuroraLogo_prev_ui.png'; 
import { loginUser } from '../Service/api';  
import { GOOGLE_CLIENT_ID } from '../config';
import { GoogleResponse } from '../Types/types';

const validationSchema = Yup.object({
    username: Yup.string()
        .min(2, 'Username is too short - should be 2 chars minimum.')
        .max(20, 'Username is too long - should be 20 chars maximum.')
        .required('Username is required.'),
    password: Yup.string()
        .min(8, 'Password is too short - should be 8 chars minimum.')
        .matches(/[A-Z]/, 'Password must contain at least one uppercase letter.')
        .matches(/\d/, 'Password must contain at least one digit.')
        .matches(/[+\-*&^]/, 'Password must contain at least one special character (+, -, *, &, ^).')
        .required('Password is required.'),
});


const responseGoogle = (response: GoogleResponse) => {
    console.log(response);
};

function LoginPage() {
    const formik = useFormik({
        initialValues: {
            username: '',
            password: '',
        },
        validationSchema: validationSchema,
        onSubmit: async (values) => {
            try {
                const user = await loginUser(values);
                console.log(user);
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
            cookiePolicy={'single_host_origin'}
            render={renderProps => (
                <button onClick={renderProps.onClick} disabled={renderProps.disabled}>
                    <img src='https://upload.wikimedia.org/wikipedia/commons/5/53/Google_%22G%22_Logo.svg' alt='Google logo' width='20' />
                    <span>Sign in with Google</span>
                </button>
            )}
        />
    );

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
                        {renderGoogleLoginButton()}
                    </div>
                </form>
            </div>
        </div>
    );
}

export default LoginPage;

