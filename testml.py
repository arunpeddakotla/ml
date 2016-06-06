import pandas as pd
from bs4 import BeautifulSoup
import re
import nltk
from sklearn.feature_extraction.text import CountVectorizer
import scipy
from sklearn.multiclass import OneVsRestClassifier
import sklearn.svm as svm
import numpy as np
from sklearn.multiclass import OneVsRestClassifier
from sklearn.svm import LinearSVC


def main():
    train = pd.read_excel("traindata.xlsx")
    test = pd.read_excel("test.xlsx")
    print test
    # print train
    # Initialize the BeautifulSoup object on a single movie review
    example1 = BeautifulSoup(train["Question"][0], "html.parser")

    # Print the raw review and then the output of get_text(), for
    # comparison
    # print train["Question"][0]
    # print example1.get_text()

    #print review_to_words(train["Question"][0])
    num_reviews = train["Question"].size
    labels = train["Category"]
    #print Y
    # Initialize an empty list to hold the clean reviews
    clean_train_reviews = []
    testData = []
    Y = []

    for i in xrange(0, num_reviews):
        Y.append(train["Category"][i] - 1)

    # Loop over each review; create an index i that goes from 0 to the length
    # of the movie review list
    for i in xrange(0, num_reviews):
        # Call our function for each one, and add the result to the list of
        # clean reviews
        clean_train_reviews.append(review_to_words(train["Question"][i]))

    for i in xrange(0, test["Question"].size):
        # Call our function for each one, and add the result to the list of
        # clean reviews
        testData.append(review_to_words(test["Question"][i]))

    #print clean_train_reviews
    vectorizer = CountVectorizer(analyzer="word", \
                                 tokenizer=None, \
                                 preprocessor=None, \
                                 stop_words=None, \
                                 max_features=5000)

    # fit_transform() does two functions: First, it fits the model


    # and learns the vocabulary; second, it transforms our training data
    # into feature vectors. The input to fit_transform should be a list of
    # strings.
    train_data_features = vectorizer.fit_transform(clean_train_reviews)
    test_data_features = vectorizer.transform(testData).toarray()

    # Numpy arrays are easy to work with, so convert the result to an
    # array
    train_data_features = train_data_features.toarray()
    #print train_data_features

    #print np.array(Y)
    clf = OneVsRestClassifier(LinearSVC()).fit(train_data_features, np.array(Y))
    #print test_data_features
    result = clf.predict(test_data_features)
    print result


def review_to_words(raw_review):
    # Function to convert a raw review to a string of words
    # The input is a single string (a raw movie review), and
    # the output is a single string (a preprocessed movie review)
    #
    # 1. Remove HTML
    review_text = BeautifulSoup(raw_review).get_text()
    #X
    # 2. Remove non-letters
    letters_only = re.sub("[^a-zA-Z]", " ", review_text)
    #
    # 3. Convert to lower case, split into individual words
    words = letters_only.lower().split()
    #
    # 4. In Python, searching a set is much faster than searching
    #   a list, so convert the stop words to a set
    #nltk.download()
    from nltk.corpus import stopwords  # Import the stop word list
    #print stopwords.words("english")
    stops = set(stopwords.words("english"))
    #
    # 5. Remove stop words
    meaningful_words = [w for w in words if not w in stops]
    #
    # 6. Join the words back into one string separated by space,
    # and return the result.
    return " ".join(meaningful_words)


if __name__ == "__main__":
    main()
