from locust import HttpLocust, TaskSet, events
import logging, os, time
from logging.handlers import RotatingFileHandler

def get_shareholder_list(l):
    l.client.get("/hc.txt")

class ShareholderList(TaskSet):
    tasks = {get_shareholder_list: 1}

    #def on_start(self):
    #    login(self)

class WebsiteUser(HttpLocust):

    host = "http://localhost:8550"
    task_set = ShareholderList
    min_wait = 5000 # 50000
    max_wait = 9000 # 300000

BACK_UP_COUNT = 20
MAX_LOG_BYTES = 1024 * 1024 * 5
LOG_PATH = "C:\Projects\Performance Testing"

def request_success_handler(request_type, name, response_time, response_length):

    msg = ' | '.join([str(request_type), name, str(response_time), str(response_length)])
    success_logger.info(msg)

events.request_success += request_success_handler
filename = "performance_stats.csv"
request_success_handler = RotatingFileHandler(filename=os.path.join(LOG_PATH, filename), maxBytes=MAX_LOG_BYTES*10, backupCount=BACK_UP_COUNT, delay=1)

formatter = logging.Formatter('%(asctime)s | %(name)s | %(levelname)s | %(message)s', datefmt='%Y-%m-%d %H:%M:%S')
formatter.converter = time.gmtime
request_success_handler.setFormatter(formatter)

success_logger = logging.getLogger('request.success')
success_logger.propagate = False
success_logger.addHandler(request_success_handler)
