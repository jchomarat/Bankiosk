import React, { Fragment } from 'react';

class LoginStatus extends React.Component {
    
    logout = () => {
        this.props.onLoginStatusChanged(null);
    }

    render ()   {
        if (this.props.loginStatus === "notlogged") {
            return (
                <Fragment>
                    <h4>Not logged</h4>
                </Fragment>
            );
        }
        else {
            return (
                <Fragment>
                    <h4>
                        Welcome {this.props.userInfoLoggedIn.FirstName}
                        <span className="pl-3"><a href="#" onClick={this.logout}>Logout</a></span>
                        
                    </h4>
                </Fragment>
            );
        }
        
    }
}

export default LoginStatus;