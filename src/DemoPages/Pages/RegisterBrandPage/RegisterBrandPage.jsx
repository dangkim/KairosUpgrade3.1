import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
//import img from '../assets/images/signup-image.jpg'
import city from '../../../assets/utils/images/originals/city.jpg'
import citynights from '../../../assets/utils/images/originals/citynights.jpg'
import citydark from '../../../assets/utils/images/originals/citydark.jpg'
import Slider from "react-slick";
import { brandActions } from '../../../_actions';
import Select from 'react-select';
import new_logo from '../../../assets/utils/images/originals/new_logo.png'
import { createLocations } from '../../../_models/CommonModels';
import NumberFormat from 'react-number-format';
import PasswordValidator from 'password-validator';

class RegisterBrandPage extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            brand: {
                //displayText:'',
                fullName: '',
                email: '',
                brandName: '',
                businessAreas: '',
                phone: '',
                password: '',
                repeatPassword: '',
                location: 'TPHCM',
            },
            selectedOptionLocation: { value: "TPHCM", label: "TPHCM" },
            submitted: false
        };

        this.handleOptionLocationChange = this.handleOptionLocationChange.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handlePassword = this.handlePassword.bind(this);
        this.handleRepeatPassword = this.handleRepeatPassword.bind(this);
    }

    handleOptionLocationChange = selectedOptionLocation => {
        this.setState({ selectedOptionLocation });
        //console.log(`Option selected:`, selectedOptionLocation);
    };

    handleChange(event) {
        const { name, value } = event.target;
        const { brand } = this.state;
        this.setState({
            brand: {
                ...brand,
                [name]: value
            }
        });
    }

    handleSubmit(event) {
        event.preventDefault();
        this.setState({ submitted: true });
        const { brand, selectedOptionLocation } = this.state;
        const { dispatch, users } = this.props;

        if (brand.fullName
            && brand.email
            && brand.brandName
            && brand.businessAreas
            && brand.phone
            && selectedOptionLocation
            && brand.password
            && brand.repeatPassword) {

            const userType = {
                UserName: brand.email,
                Email: brand.email,
                Password: brand.password,
                ConfirmPassword: brand.repeatPassword,
                IsFluencer: false,
                IsBrand: true
            }

            const brandType = {
                ContentItemId: '',
                ContentItemVersionId: '',
                ContentType: 'Brand',
                DisplayText: brand.brandName + ';' + brand.email + ';' + brand.fullName,
                Latest: false,
                Published: false,
                ModifiedUtc: null,
                PublishedUtc: null,
                CreatedUtc: null,
                Owner: 'admin',
                Author: 'admin',
                Brand: {
                    FullName: {
                        Text: brand.fullName
                    },
                    Email: {
                        Text: brand.email
                    },
                    BrandName: {
                        Text: brand.brandName
                    },
                    BusinessAreas: {
                        Text: brand.businessAreas
                    },
                    Phone: {
                        Text: brand.phone
                    },
                    Password: {
                        Text: brand.password
                    },
                    Location: {
                        Text: selectedOptionLocation.value
                    }
                },
                TitlePart: {
                    Title: 'Brand ' + brand.brandName,
                }
            }

            dispatch(brandActions.register(brandType, userType));
        }
    }

    componentDidMount() {
        const { dispatch } = this.props;
    }

    handleRepeatPassword = repeatPassword => {
        const { brand } = this.state;
        if (brand.password !== repeatPassword) {
            return false;
        } else {
            return true;
        };
    }

    handlePassword = password => {
        const { brand } = this.state;

        // Create a schema
        var schema = new PasswordValidator();
        schema
            .is().min(8)
            .is().max(20)
            .has().uppercase()
            .has().lowercase()
            .has().digits()
            .has().not().spaces()
            .has().symbols()

        const valid = schema.validate(password)
        return valid;
        //this.setState({ password });
        //console.log(`Option selected:`, selectedOptionLocation);
    };

    render() {
        const { brands } = this.props;
        const { brand, selectedOptionLocation, submitted } = this.state;
        const options = createLocations();
        const settings = {
            dots: true,
            infinite: true,
            speed: 500,
            slidesToShow: 1,
            slidesToScroll: 1
        };
        return (
            <div className="app-container app-theme-white body-tabs-shadow">
                <div className="app-container">
                    <div className="h-100">
                        <div className="h-100 no-gutters row">
                            <div className="h-100 d-md-flex d-sm-block bg-white justify-content-center align-items-center col-md-12 col-lg-8">
                                <div className="mx-auto app-login-box col-sm-12 col-md-10 col-lg-9">
                                    <div style={{ marginBottom: '3rem', width: '97px', height: '23px' }}>
                                        <img src={new_logo} alt="Kols Viet" />
                                    </div>
                                    <h4>
                                        <div>Welcome,</div>
                                        <span>It only takes a <span className="text-success">few seconds</span> to create your account</span></h4>
                                    <div>
                                        <form className="" onSubmit={this.handleSubmit} id="register-form">
                                            <div className="form-row">
                                                <div className="col-md-6">
                                                    <div className="position-relative form-group">
                                                        <label htmlFor="name" className="">
                                                            <span className="text-danger">*</span> Full Name</label>
                                                        <input type="text" name="fullName" id="fullName" placeholder="Your Full Name" value={brand.fullName} onChange={this.handleChange} className="form-control" />
                                                        {
                                                            submitted && !brand.fullName &&
                                                            <div className="help-block text-danger">Full Name is required</div>
                                                        }
                                                    </div>
                                                </div>
                                                <div className="col-md-6">
                                                    <div className="position-relative form-group">
                                                        <label htmlFor="email" className="">
                                                            <span className="text-danger">*</span> Email
                                                        </label>
                                                        <input type="email" name="email" id="email" placeholder="Your Email" value={brand.email} onChange={this.handleChange} className="form-control" />
                                                        {
                                                            submitted && !brand.email &&
                                                            <div className="help-block text-danger">Email is required</div>
                                                        }
                                                    </div>
                                                </div>
                                                <div className="col-md-6">
                                                    <div className="position-relative form-group">
                                                        <label htmlFor="location" className="">
                                                            <span className="text-danger">*</span> Location</label>
                                                        <Select
                                                            maxMenuHeight={200}
                                                            value={selectedOptionLocation}
                                                            onChange={this.handleOptionLocationChange}
                                                            isMulti={false}
                                                            placeholder="Location..."
                                                            options={options} />
                                                        {
                                                            submitted && !selectedOptionLocation &&
                                                            <div className="help-block text-danger">Location is required</div>
                                                        }
                                                    </div>
                                                </div>
                                                <div className="col-md-6">
                                                    <div className="position-relative form-group">
                                                        <label htmlFor="name" className="">
                                                            <span className="text-danger">*</span> Phone
                                                        </label>
                                                        <NumberFormat className="form-control" name="phone" id="phone" format="+84 (####) ###-###" mask="_" value={brand.phone} onChange={this.handleChange} placeholder="Your Phone" />
                                                        {
                                                            submitted && !brand.phone &&
                                                            <div className="help-block text-danger">Phone is required</div>
                                                        }
                                                    </div>
                                                </div>
                                                <div className="col-md-6">
                                                    <div className="position-relative form-group">
                                                        <label htmlFor="name" className="">
                                                            <span className="text-danger">*</span> Brand Name
                                                        </label>
                                                        <input type="text" name="brandName" id="brandname" placeholder="Your brand Name" value={brand.brandName} onChange={this.handleChange} className="form-control" />
                                                        {
                                                            submitted && !brand.brandName &&
                                                            <div className="help-block text-danger">Brand Name is required</div>
                                                        }
                                                    </div>
                                                </div>
                                                <div className="col-md-6">
                                                    <div className="position-relative form-group">
                                                        <label htmlFor="name" className="">
                                                            <span className="text-danger">*</span> Business areas
                                                        </label>
                                                        <input type="text" name="businessAreas" id="businessAreas" placeholder="Ex: Sport, Fashion" value={brand.businessAreas} onChange={this.handleChange} className="form-control" />
                                                        {
                                                            submitted && !brand.businessAreas &&
                                                            <div className="help-block text-danger">Business areas is required</div>
                                                        }
                                                    </div>
                                                </div>
                                                <div className="col-md-6">
                                                    <div className="position-relative form-group">
                                                        <label htmlFor="pass" className="">
                                                            <span className="text-danger">*</span> Password</label>
                                                        <input type="password" id="password" placeholder="Password" name="password" value={brand.password} onChange={this.handleChange} className="form-control" />
                                                        {
                                                            submitted && !brand.password &&
                                                            <div className="help-block text-danger">Password is required</div>
                                                        }
                                                        {
                                                            submitted && !this.handlePassword(brand.password) &&
                                                            <div className="help-block text-danger">Password minimum length 8, must have uppercase, lowercase, digits, special letters and not have space</div>
                                                        }
                                                    </div>
                                                </div>
                                                <div className="col-md-6">
                                                    <div className="position-relative form-group">
                                                        <label htmlFor="pass" className="">
                                                            <span className="text-danger">*</span> Repeat Password</label>
                                                        <input type="password" id="repeatPassword" placeholder="Repeat Password" name="repeatPassword" value={brand.repeatPassword} onChange={this.handleChange} className="form-control" />
                                                        {
                                                            submitted && !brand.repeatPassword &&
                                                            <div className="help-block text-danger">Repeat Password is required</div>
                                                        }
                                                        {
                                                            submitted && !this.handlePassword(brand.repeatPassword) &&
                                                            <div className="help-block text-danger">Password incorrect</div>
                                                        }
                                                        {
                                                            submitted && !this.handleRepeatPassword(brand.repeatPassword) &&
                                                            <div className="help-block text-danger">Password is not matched</div>
                                                        }
                                                    </div>
                                                </div>
                                            </div>
                                            <div className="mt-3 position-relative form-check"><input name="check" id="exampleCheck" type="checkbox" className="form-check-input" /><label htmlFor="exampleCheck" className="form-check-label">Accept our <a href="javascript:void(0);">Terms
                                        and Conditions</a>.</label></div>
                                            <div className="mt-4 d-flex align-items-center">
                                                <h5 className="mb-0">Already have an account?
                                                <Link to="/pages/loginpage" className="text-primary">Sign in</Link>
                                                </h5>
                                                <div className="ml-auto">
                                                    {/* <button className="btn-wide btn-pill btn-shadow btn-hover-shine btn btn-primary btn-lg">Create Account</button> */}
                                                    <input type="submit" name="signup" id="signup" className="btn-wide btn-pill btn-shadow btn-hover-shine btn btn-primary btn-lg" value="Create Account" />
                                                    {
                                                        brands.registering &&
                                                        <img src="data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==" />
                                                    }
                                                </div>
                                            </div>
                                        </form>
                                    </div>
                                </div>
                            </div>
                            <div className="d-none d-lg-block col-lg-4">
                                <div className="slider-light">
                                    <div className="slick-slider slick-initialized">
                                        <Slider {...settings}>
                                            <div>
                                                <div className="position-relative h-100 d-flex justify-content-center align-items-center bg-plum-plate" tabIndex="-1">
                                                    <div className="slide-img-bg" style={{ backgroundImage: `url(${city})` }}></div>
                                                    <div className="slider-content"><h3>Active User Rate</h3>
                                                        <p>giúp nhãn hàng chọn đúng Influencer khi vấn nạn “follower ảo” ngày càng cao.</p></div>
                                                </div>
                                            </div>
                                            <div>
                                                <div className="position-relative h-100 d-flex justify-content-center align-items-center bg-premium-dark" tabIndex="-1">
                                                    <div className="slide-img-bg" style={{ backgroundImage: `url(${citynights})` }}></div>
                                                    <div className="slider-content"><h3>Sentiment Score</h3>
                                                        <p>Chỉ số phản ánh cảm xúc của người dùng thông qua các phản hồi tích cực/tình cảm/ thái độ của followers đối với các nội dung do Influencer chia sẻ.</p></div>
                                                </div>
                                            </div>
                                            <div>
                                                <div className="position-relative h-100 d-flex justify-content-center align-items-center bg-sunny-morning" tabIndex="-1">
                                                    <div className="slide-img-bg" style={{ backgroundImage: `url(${citydark})` }}></div>
                                                    <div className="slider-content"><h3>Influence Score</h3>
                                                        <p>Là chỉ số tổng hợp từ các chỉ số trên và đã được thử nghiệm kỹ càng trên nhiều thuật toán. Vì vậy, Influence Score có thể phản ánh gần đúng nhất mức độ ảnh hưởng của Influencer trên từng chủ đề.</p></div>
                                                </div>
                                            </div>
                                        </Slider>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

function mapStateToProps(state) {
    const { locations, brands, users } = state;
    return {
        brands,
        locations,
        users
    };
}

const connectedRegisterBrandPage = connect(mapStateToProps)(RegisterBrandPage);
export { connectedRegisterBrandPage as RegisterBrandPage };