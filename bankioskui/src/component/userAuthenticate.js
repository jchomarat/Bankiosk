import React, { Fragment } from 'react';
import Button from 'react-bootstrap/Button';
import ButtonToolbar from 'react-bootstrap/ButtonToolbar';
import ButtonGroup from 'react-bootstrap/ButtonGroup';
import Dropdown from 'react-bootstrap/Dropdown';
import Card from 'react-bootstrap/Card';
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import { Loader } from 'react-overlay-loader';
import Authenticate from '../helpers/Authenticate';
import ApiCalls from '../helpers/ApiCalls';

import './../css/index.css';

class UserInfo extends React.Component {

    formatDateCc = (input) => {
        let date = new Date(input);
        return (`${date.getMonth() + 1} - ${date.getFullYear()}`);
    }

    formatDateAppointment = (input) => {
        let date = new Date(input)
        return (`${date.toDateString()} at ${date.getHours()}:${date.getMinutes()}`);
    }

    render() {

        if (!this.props.show) {
            return ("");
        }
        else {
            return (
                <Fragment>
                    <Card body>
                        <h4><b>Credit card information</b></h4>
                        <ul>
                            <li><b>Card expiration date:</b> {this.formatDateCc(this.props.user.card.ExpirationDate)}</li>
                            <li><b>Number total of operations:</b> {this.props.user.card.NumberOperation}</li>
                            <li><b>Number of PIN retrie(s) left:</b> {this.props.user.card.PinRetryLeft}</li>
                        </ul>
                    </Card>

                    <Card body className="mt-2">
                        <h4><b>Upcoming appointments</b></h4>
                        <ul>
                            {this.props.user.appointments.map((a) => 
                                <li key={a.Subject}>{a.Subject} with <b>{a.AdvisorDetails.FirstName} {a.AdvisorDetails.Surname}</b> on {this.formatDateAppointment(a.AppointmentTime)}</li>
                            )}
                        </ul>
                    </Card>

                    <Card body className="mt-2">
                        <h4><b>Documents to retreive at the main desk</b></h4>
                        <ul>
                            {this.props.user.documents.map((d) => 
                                <li key={d}>{d}</li>
                            )}
                        </ul>
                    </Card>
                </Fragment>
            );
        }
    }
}

class UserCardUnknown extends React.Component {

    render() {
        if (!this.props.show) {
            return ("");
        }
        else {
            return (
                <Fragment>
                    <div>
                        I could not retrieve your card information.
                    </div>
                </Fragment>
            );
        }
    }
}

class CardAuthenticate extends React.Component {

    cardAuthenticate = async () => {
        // Show loading
        this.props.onCardAuthenticate();
        let authenticate = new Authenticate();
        authenticate.CreditCard(this.props.user.RowKey)
            .then (data => {
                if (data.status) {
                    this.props.onCardAuthenticated(data.data);
                }
                else {
                    this.props.onCardAuthenticated(null);
                }
            });
    };

    render() {
        if (!this.props.show) {
            return ("");
        }
        else {
            return (
                <Fragment>
                    <Card body>
                        <h4>Hello <b>{this.props.user.FirstName} </b></h4>
                        <div>
                            You need now to confirm your identity:
                            <ol>
                                <li>Click on the button below</li>
                                <li>Then, you have one minute to put your card on the reader below</li>
                            </ol>
                            <Button onClick={this.cardAuthenticate}>Start</Button>
                        </div>
                    </Card>
                </Fragment>
            );
        }
    }

}

class UserFaceUnknown extends React.Component {

    retry = () => {
        this.props.retry();
    }

    render() {
        return (
            this.props.show ? (
                <Fragment>
                    <Card body>
                        <h4>Hello</h4>
                        <div>
                            Sorry, you are unknown
                        </div>
                        <UserFunFact show={this.props.funFact !== null} funFact={this.props.funFact} />
                        <Button onClick={this.retry}>Try again</Button>
                    </Card>
                </Fragment>
            ) : (
                <Fragment></Fragment>
            )
        );
    }

}

class UserFunFact extends React.Component {
    render() {
        if (this.props.funFact !== null && this.props.funFact.TagName == "hat") {
            return (
                <div>However, you have a nice hat, it suits you perfectly!</div>
            );
        }

        if (this.props.funFact !== null && this.props.funFact.TagName == "glasses") {
            return (
                <div>However, these glasses highlight your beautiful eyes!</div>
            );
        }

        if (this.props.funFact !== null && this.props.funFact.TagName == "tie") {
            return (
                <div>However, this tie, sir, is amazing. The color is perfect with the suit!</div>
            );
        }

        return ("");
    }
}

class FaceAuthenticate extends React.Component {
    
    constructor(props) {
        super(props)
        this.state = {
            videoInputs: [],
            takePhotoButtonDisabled: true,
        }

        this.enumerateVideoInputDevices();
    }

    enumerateVideoInputDevices = async () => {
        try {
            await navigator.mediaDevices.getUserMedia({audio: false, video: true});
            const devices = await navigator.mediaDevices.enumerateDevices();
            const d = [];
            devices.map(device => {
                if (device.kind === "videoinput") {
                    d.push({
                        label: device.label,
                        id: device.deviceId
                    })
                }
                return true;
            });

            this.setState({
                videoInputs: d
            });
            
        } catch (e) {
            console.error(e)
        }
    }

    captureStream = async (event) => {
        const video = document.querySelector("#videoElement");
        const deviceId = event.target.getAttribute('deviceid');
        try {
            const mediaStream = await navigator.mediaDevices.getUserMedia(
                { 
                    audio: false, 
                    video: {
                      deviceId: {
                        exact: deviceId
                      }
                    }
                });
            video.srcObject = mediaStream;
            this.setState({
                takePhotoButtonDisabled: false
            })
        } catch (e) {
            console.error(e);
        }
        video.onloadedmetadata = async function(event) {
            try {
              await video.play();
            } catch (e) {
                console.error(e)
            }
        }
    }

    faceAuthenticate = async () => {
        // Show loading bar
        this.props.onFaceAuthenticate();
        const canvas = document.querySelector("#canvas");
        const video = document.querySelector("#videoElement");
        canvas.getContext('2d').drawImage(video, 0, 0, canvas.width, canvas.height);

        // Send the blob to the authentication service
        canvas.toBlob((blob) => {
            let authenticate = new Authenticate();
            authenticate.Face(blob)
                .then (data => {
                    if (data.status) {
                        this.props.onFaceAuthenticated(data.data);
                    }
                    else {
                        this.props.onFaceAuthenticated(null, data.funFact);
                    }
                })
        })      
    };

    render() {
        if (!this.props.show) {
            return ("");
        }
        else {
            return (
                <Container>
                    <Row>
                        <Col md={3}>
                            <ButtonToolbar>
                                <ButtonGroup className="mr-2">
                                    <Dropdown>
                                        <Dropdown.Toggle variant="success" id="dropdown-basic">
                                            [Select camera]
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu>
                                            {this.state.videoInputs.map((input) => <Dropdown.Item onClick={this.captureStream} key={input.id} deviceid={input.id}>{input.label}</Dropdown.Item>)}   
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </ButtonGroup>
                                <ButtonGroup  className="mt-5">
                                    <Button className="btn-lg" onClick={this.faceAuthenticate} disabled={this.state.takePhotoButtonDisabled}>Take photo</Button>
                                </ButtonGroup>
                            </ButtonToolbar>
                        </Col>
                        <Col md={9} className="align-self-center">
                            <video autoPlay={false} id="videoElement" className="userPhoto-videoElement"></video>                    
                            <canvas className="userPhoto-canvas" id="canvas" /><br />
                        </Col>
                    </Row>
                </Container>
            );
        }
    }
}

class UserAuthenticate extends React.Component {

    constructor(props) {
        super(props)
        this.state = {
            step: "faceauthenticate",
            showLoadingOverlay: false,
            userInfo: null, //{FirstName, Surname, RowKey} then after cards/documents/appointments
            userFunFact: null // If user unknown, some fun fact may appear
        }
    }

    onAsync = () => {
        this.setState({ showLoadingOverlay: true })
    }

    retry = () => {
        this.setState({
            step: "faceauthenticate",
            showLoadingOverlay: false
        });
    }

    onFaceAuthenticated = (user, funFact) => {
        this.setState({ 
            showLoadingOverlay: false,
            userInfo: user,
            step: (user !== null ? "cardauthenticate" : "userfaceunknown"),
            userFunFact: funFact
            }, () => {
                // Tell parent about it
                if (user !== null)
                    this.props.onLoginStatusChanged(user);
        });
        
    }

    onCardAuthenticated = (card) => {
        // If user is not recognized, show it
        if (card === null) {
            this.setState({ 
                showLoadingOverlay: false,
                step: "usercardunknown"
            });
        }
        else {
            // User is autenticated, retreive his todo (appointments and/or documents to pick)
            let api = new ApiCalls();
            let usr = this.state.userInfo;
            usr.card = card;
            api.Get(api.userAppointments(this.state.userInfo.RowKey))
                .then(response => response.json())
                .then(data => {
                    if(data.status) {
                       let appointments = data.value.map(a => {return JSON.parse(a);});
                       usr.appointments = appointments;
                    }
                    api.Get(api.userDocuments(this.state.userInfo.RowKey))
                        .then(response => response.json())
                        .then(data => {
                            if(data.status) {
                                let documents = data.value.map(d => {return d;});
                                usr.documents = documents;
                            }
                            this.setState({ 
                                showLoadingOverlay: false,
                                userInfo: usr,
                                step: "userinfo"
                            });
                        });   
                })
        }        
    }

    render() {
        return (
            <Container>
                <Row>
                    <Col md={12} className="mt-2 justify-content-center">
                        <Loader fullPage loading={this.state.showLoadingOverlay} containerStyle={{background: "rgba(255, 255, 255, 0.9)"}}/>
                        <FaceAuthenticate 
                            onFaceAuthenticate={this.onAsync}
                            onFaceAuthenticated={this.onFaceAuthenticated} 
                            show={this.state.step === "faceauthenticate"}
                        />
                        <UserFaceUnknown show={this.state.step === "userfaceunknown"} funFact={this.state.userFunFact} retry={this.retry} />
                        <CardAuthenticate 
                            onCardAuthenticate={this.onAsync}
                            onCardAuthenticated={this.onCardAuthenticated} 
                            show={this.state.step === "cardauthenticate"} 
                            user={this.state.userInfo}
                        />
                        <UserCardUnknown show={this.state.step === "usercardunknown"} />
                        <UserInfo 
                            show={this.state.step === "userinfo"} 
                            user={this.state.userInfo} 
                        />
                    </Col>
                </Row>
            </Container>
        );
    }
}

export default UserAuthenticate;