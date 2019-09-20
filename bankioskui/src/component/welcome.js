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
                <Button className="mt-2 p-2 w-75 btn-lg" onClick={this.login}>Login</Button>
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
                    <Col md={9}>
                        <Actions />
                    </Col>
                    <Col md={1} className="align-self-center">
                    <h4>
                        <b>Or</b>
                    </h4>
                    </Col>
                    <Col md={2} className="align-self-center justify-content-center">
                        <Login onLoginClick={this.onLoginClick} />
                    </Col>
                </Row>
            </Container> 
        );
    }
}

export default Welcome;
