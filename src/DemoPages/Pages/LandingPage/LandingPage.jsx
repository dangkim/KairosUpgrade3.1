import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import bg3 from '../../../assets/utils/images/bg3.jpg'
import iphone3 from '../../../assets/utils/images/iphone3.png'
import m4 from '../../../assets/utils/images/m4.gif'
import m31 from '../../../assets/utils/images/m31.gif'
import mac from '../../../assets/utils/images/mac.png'
import m2 from '../../../assets/utils/images/m2.png'
// import mac from '../../../assets/utils/images/mac.png'
import big_logo from '../../../assets/utils/images/big_logo.jpg'
import bogolive from '../../../assets/utils/images/bogolive.png'
import clients from '../../../assets/utils/images/clients.jpg'
import momo from '../../../assets/utils/images/momo.png'
import nct from '../../../assets/utils/images/nct.png'
import sofy from '../../../assets/utils/images/sofy.png'
import vnngaynay from '../../../assets/utils/images/vnngaynay.png'
import gkitchen from '../../../assets/utils/images/gkitchen.png'
import vinaphone from '../../../assets/utils/images/vinaphone.png'
import home_33 from '../../../assets/utils/images/home_33.jpg'
import home_22 from '../../../assets/utils/images/home_22.jpg'
import home_11 from '../../../assets/utils/images/home_11.jpg'

// import home_33 from '../../../assets/utils/images/home_33.jpg'
// import home_33 from '../../../assets/utils/images/home_33.jpg'
import { userActions } from '../../../_actions';

class LandingPage extends Component {
    constructor(props) {
        super(props);

        // reset login status
        //this.props.dispatch(userActions.logout());

        this.state = {
            userName: '',
            email: '',
            password: '',
            submitted: false,
            token: ''
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(e) {
        const { name, value } = e.target;
        this.setState({ [name]: value });
    }

    handleSubmit(e) {
        e.preventDefault();

        this.setState({ submitted: true });
        const { userName, email, password } = this.state;
        const { dispatch } = this.props;
        if (userName && password) {
            dispatch(userActions.getToken(userName, password));
        }
    }

    render() {
        const { loggingIn, token } = this.props;
        const { userName, password, submitted } = this.state;
        const settings = {
            dots: true,
            infinite: true,
            speed: 500,
            slidesToShow: 1,
            slidesToScroll: 1
        };
        return (
            <div className="landing-page landing-page1">
                <nav className="navbar navbar-transparent navbar-top" role="navigation">
                    <div className="container">
                        <div className="navbar-header">
                            <div className="logo-container">
                                <div className="logo" style={{ border: 'none', width: '150px', borderRadius: 'unset' }}>
                                    <img src={big_logo} alt="Kols Viet" />
                                </div>
                                {/* <div className="brand">
                                        Kols Viet
                                    </div> */}
                            </div>
                        </div>
                    </div>
                </nav>
                <div className="wrapper">
                    <div className="parallax filter-gradient blue" data-color="blue">
                        <div className="parallax-background">
                            <img className="parallax-background-image" src={bg3} />
                        </div>
                        <div className="container">
                            <div className="row">
                                <div className="col-md-5 hidden-xs">
                                    <div className="parallax-image">
                                        {/* <img className="phone" src={iphone3} style={{ marginTop: '20px' }} /> */}
                                    </div>
                                </div>
                                <div className="col-md-6 col-md-offset-1">
                                    <div className="description">
                                        <h3>THẤU HIỂU KHÁCH HÀNG</h3>
                                        <h3>AM HIỂU THỊ TRƯỜNG.</h3>
                                        <br />
                                        {/* <h5>Be amazed by the best looking bootstrap landing page on the web! Your new app deserves an amazing page to show all of its features. Clear visual, light colours and beautifully aligned elements - they all try to make the users aware of your great app features!</h5> */}
                                        <div className="">
                                            <Link to="/pages/loginpage" className="btn btn-fill btn-info">Get Free Access</Link>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="section section-gray section-clients">
                        <div className="container text-center">
                            <h4 className="header-text">GIỚI THIỆU DOANH NGHIỆP</h4>
                            <p>
                                KOLV tự tin cung cấp giải pháp Influencer Marketing hiệu quả đối với các nhãn hàng và mang đến nguồn thu nhập thỏa đáng cùng cơ hội trải nghiệm sản phẩm chất lượng cho Influencer. Với đội ngũ nhân sự giàu kinh nghiệm trong lĩnh vực truyền thông cùng nền tảng công nghệ hiện đại được đầu tư và phát triển hơn 2 năm.
                                <br />
                            </p>
                            <div className="logos">
                                <ul className="list-unstyled">
                                    <li ><img src={bogolive} /></li>
                                    <li ><img src={gkitchen} /></li>
                                    <li ><img src={momo} /></li>
                                    <li ><img src={nct} /></li>
                                    <li ><img src={sofy} /></li>
                                    <li ><img src={vnngaynay} /></li>
                                    <li ><img src={vinaphone} /></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div className="section section-presentation" style={{ backgroundImage: `url(${m31})`, minHeight: '840px' }}>
                        <div className="container">
                            <div className="row">
                                <div className="col-md-6">
                                    <div className="description">
                                        <h6 className="header-text">CÔNG NGHỆ HIỆN ĐẠI</h6>
                                        <p>KOLV không chỉ được kế thừa các công nghệ dẫn đầu thị trường về xử lý dữ liệu lớn và phân tích ngôn ngữ tự động mà còn được định vị sẽ mang đến các giá trị nổi bật và khác biệt</p>
                                    </div>
                                </div>
                                {/* <div className="col-md-5 col-md-offset-1 hidden-xs">
                                    <img src={m31} />
                                </div> */}
                            </div>
                        </div>
                    </div>
                    <div className="section section-presentation">
                        <div className="container" style={{ backgroundImage: `url(${m2})`, minHeight: '700px' }}>
                            <div className="row" >
                                <div className="col-md-4">
                                </div>
                                <div className="col-md-8">
                                    <div className="description">
                                        <h6 className="header-text">KẾT NỐI DÀI LÂU</h6>
                                        <p>&bull;Kho dữ liệu lớn nhất: 10,000 Influencers và tiếp tục mở rộng</p>
                                        <p>&bull;Hệ thống phân tích thống kê dữ liệu toàn diện - real-time</p>
                                    </div>
                                </div>
                            </div>
                            <div className="row" >
                                <div className="col-md-7">
                                </div>
                                <div className="col-md-5">
                                    <div className="description">
                                        <p>&bull;Các gói dịch vụ hỗ trợ triển khai chiến dịch toàn diện</p>
                                        <p>&bull;Và hơn hết KOLV là cấu nối nhịp nhàng giữa nhãn hàng và các Influencer để tạo nên hiệu quả nhất cho chiến dịch</p>
                                        <Link to="/pages/loginpage" className="btn btn-fill btn-info">Get Free Access</Link>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>                    
                    <div className="section section-features">
                        <div className="container">
                            <h4 className="header-text text-center">ĐIỀU GÌ LÀM CHO KOLV KHÁC BIỆT</h4>
                            <div className="row">
                                <div className="col-md-4">
                                    <div className="card card-blue">
                                        <div className="icon">
                                            <i className="pe-7s-note2"></i>
                                        </div>
                                        <div className="text">
                                            <h4>Active User Rate</h4>
                                            <p>Giúp nhãn hàng chọn đúng Influencer khi vấn nạn “follower ảo” ngày càng cao</p>
                                            <h4 style={{ marginTop: '0px' }}>Relevance Score</h4>
                                            <p style={{ marginTop: '20px' }}>Thể hiện mức độ phù hợp của Influencer trong từng lĩnh vực, ngành hàng khác nhau.</p>
                                        </div>
                                    </div>
                                </div>
                                <div className="col-md-4">
                                    <div className="card card-blue">
                                        <div className="icon">
                                            <i className="pe-7s-graph1"></i>
                                        </div>
                                        <h4>Sentiment Score</h4>
                                        <p>Chỉ số cảm xúc tích cực: phản ánh cảm xúc của người dùng thông qua các phản hồi tích cực, tình cảm, thái độ của followers đối với các nội dung do Influencer chia sẻ.</p>
                                    </div>
                                </div>
                                <div className="col-md-4">
                                    <div className="card card-blue">
                                        <div className="icon">
                                            <i className="pe-7s-bell"></i>
                                        </div>
                                        <h4>Influence Score</h4>
                                        <p>Chỉ số ảnh hưởng: tổng hợp từ các chỉ số trên và đã được thử nghiệm kỹ càng trên nhiều thuật toán. Vì vậy, Influence Score có thể phản ánh gần đúng nhất mức độ ảnh hưởng của Influencer trên từng chủ đề.</p>
                                    </div>
                                </div>
                                <div className="col-md-2">
                                </div>
                                <div className="col-md-8">
                                    <div className="card card-blue">
                                        <div className="icon">
                                            <i className="pe-7s-note2"></i>
                                        </div>
                                        <div className="text">
                                            <h4>Resonance Score</h4>
                                            <p>Chỉ số cộng hưởng: tỷ lệ tương đồng giữa nội dung bình luận với nội dung post. Trong Influencer Marketing, comment liên quan đến chủ đề, thương hiệu là một chỉ số quan trọng để hiểu rõ nhu cầu, hứng thú của followers đến sản phẩm/thông điệp cũng như giúp đánh giá hiệu quả của Influencer/Chiến dịch. Nhưng làm sao con người có thể đọc hiểu hàng ngàn comment của followers? <b>Công nghệ của KOLV làm giúp Marketers việc này.</b></p>
                                        </div>
                                    </div>
                                </div>
                                <div className="col-md-2">
                                </div>
                            </div>
                        </div>
                    </div>
                    {/* <div className="section section-testimonial">
                        <div className="container">
                            <h4 className="header-text text-center">What people think</h4>
                            <div id="carousel-example-generic" className="carousel fade" data-ride="carousel">
                                <div className="carousel-inner" role="listbox">
                                    <div className="item active">
                                        <div className="carousel-testimonial-caption">
                                            <h3>"Đây là một trong những nền tảng tuyệt vời nhất tôi từng thấy! Chúc bạn may mắn KolsViet!"</h3>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div> */}
                    <div className="section section-presentation">
                        <div className="container" style={{ backgroundImage: `url(${m4})`, minHeight: '466px' }}>
                            <div className="row">
                                <div className="col-md-6">
                                    <div className="description">
                                        <h6 className="header-text">Mọi người nghĩ gì</h6>
                                        <p>Đây là một trong những nền tảng tuyệt vời nhất tôi từng thấy! Chúc bạn may mắn KolViet!</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="section section-no-padding">
                        <div className="parallax filter-gradient blue" data-color="blue">
                            <div className="parallax-background">
                                <img className="parallax-background-image" src={bg3} />
                            </div>
                            <div className="info">
                                <h1>Try this for free!</h1>
                                <p>Beautiful places for you.</p>
                                <Link to="/pages/loginpage" className="btn btn-neutral btn-lg btn-fill">Get Free Access</Link>
                                {/* <a href="http://www.creative-tim.com/product/awesome-landing-page" className="btn btn-neutral btn-lg btn-fill">EXPLORE</a> */}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

function mapStateToProps(state) {
    const { loggingIn, token } = state.authentication;
    return {
        loggingIn,
        token
    };
}

const connectedLandingPage = connect(mapStateToProps)(LandingPage);
export { connectedLandingPage as LandingPage }; 