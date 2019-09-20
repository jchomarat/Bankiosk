import React, { Fragment } from 'react';
import Badge from 'react-bootstrap/Badge';

import LoginStatus from './loginStatus';
import Welcome from './welcome';
import UserAuthenticate from './userAuthenticate';

import './../css/index.css';

class Frame extends React.Component {

  constructor(props) {
    super(props)
    this.state = {
        loginStatus: "notlogged",
        userInfoLoggedIn: null,
        showWelcome: true,
        showUserAuthenticate: false
    }
  }

  refreshPage = () => {
    window.location.reload();
  }

  loginInitiated = () => {
    this.setState({
      showWelcome: false,
      showUserAuthenticate: true
    });
  }

  loginStatusChanged = (userLoggedIn) => {
    // If userLoggedIn is null, then it is logout or not logged yet
    if (userLoggedIn !== null) {
        this.setState({
            loginStatus: "logged",
            userInfoLoggedIn: userLoggedIn
        });
    }
    else {
        this.setState({
            loginStatus: "notlogged",
            userInfoLoggedIn: null,
            showWelcome: true,
            showUserAuthenticate: false
        });
    }
  }

  render() {

    return (
      <Fragment>
        <header>
          <div className="d-flex">
            <div className="mr-auto">
              <h1 onClick={this.refreshPage} className="hand">
                Bankiosk <Badge variant="secondary">Demo</Badge>
              </h1>
            </div>
            <div>
              <LoginStatus loginStatus={this.state.loginStatus} userInfoLoggedIn={this.state.userInfoLoggedIn} onLoginStatusChanged={this.loginStatusChanged}/>
            </div>
          </div>
        </header>

        {this.state.showWelcome &&
            <Welcome onLoginInitiated={this.loginInitiated} />
        }

        {this.state.showUserAuthenticate &&
            <UserAuthenticate onLoginStatusChanged={this.loginStatusChanged} />
        }
      </Fragment> 
    );
  }
}

export default Frame;
