import requests as req


class Client:
    def __init__(self):
        # self.base_url = 'http://localhost:5249/'
        self.base_url = 'https://localhost:7196/'
        self.verify = False
    def register(self):
        # req.post(f"{self.base_url}devices/register",json={"name":"test"},verify=self.verify)
        req.post(f"{self.base_url}devices/register",json={"name":"test"},verify='C:\\Users\\kubaa\\aspnetcore-dev.crt')


if __name__ == '__main__':
    print("Start")
    client = Client()
    client.register()