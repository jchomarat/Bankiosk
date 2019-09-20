import React, { Fragment } from 'react';
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';
import Actions from './actions';

import './../css/index.css';

class Login extends React.Component {
    
    login = () => {
        this.props.onLoginClick();
    }
    
    render() {
        return (
            <Fragment>
                <Button onClick={this.login}>Login</Button>
            </Fragment>
        );
    }
}

class Welcome extends React.Component {

    onLoginClick = () => {
        this.props.onLoginInitiated();
    }

    render() {
        return (
            <Container>
                <Row>
                    <Col md={7}>
                    <Actions />
                    </Col>
                    <Col md={1} className="align-self-center">
                    <h3>
                        <b>Or</b>
                    </h3>
                    </Col>
                    <Col md={4} className="align-self-center justify-content-center">
                    <Login onLoginClick={this.onLoginClick} />
                    </Col>
                </Row>
            </Container> 
        );
    }
}

export default Welcome;
