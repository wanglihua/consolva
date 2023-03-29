package com.huadu.shunda.data.http.login;

import com.google.gson.annotations.SerializedName;

public class LoginResponseData {
    @SerializedName("userId")
    private int userId;

    @SerializedName("userName")
    private String userName;

    @SerializedName("canExpressQuery")
    private boolean canExpressQuery;

    @SerializedName("canExpressOnShelf")
    private boolean canExpressOnShelf;

    @SerializedName("canExpressWarehouseCheck")
    private boolean canExpressWarehouseCheck;

    @SerializedName("canExpressOffShelf")
    private boolean canExpressOffShelf;

    @SerializedName("canExpressReturnOffShelf")
    private boolean canExpressReturnOffShelf;

    @SerializedName("canExpressDump")
    private boolean canExpressDump;

    @SerializedName("canExpressPacking")
    private boolean canExpressPacking;

    @SerializedName("canPackBoxQuery")
    private boolean canPackBoxQuery;

    @SerializedName("canPackBoxShip")
    private boolean canPackBoxShip;

    @SerializedName("canPhotoSignIn")
    private boolean canPhotoSignIn;

    @SerializedName("canPhotoCheckIn")
    private boolean canPhotoCheckIn;

    @SerializedName("canPhotoIssue")
    private boolean canPhotoIssue;

    @SerializedName("canPhotoPack")
    private boolean canPhotoPack;

    @SerializedName("canPhotoReturn")
    private boolean canPhotoReturn;

    @SerializedName("canPhotoShip")
    private boolean canPhotoShip;

    @SerializedName("canPhoto")
    private boolean canPhoto;

    public int getUserId() {
        return userId;
    }

    public void setUserId(int userId) {
        this.userId = userId;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public boolean isCanExpressQuery() {
        return canExpressQuery;
    }

    public void setCanExpressQuery(boolean canExpressQuery) {
        this.canExpressQuery = canExpressQuery;
    }

    public boolean isCanExpressOnShelf() {
        return canExpressOnShelf;
    }

    public void setCanExpressOnShelf(boolean canExpressOnShelf) {
        this.canExpressOnShelf = canExpressOnShelf;
    }

    public boolean isCanExpressWarehouseCheck() {
        return canExpressWarehouseCheck;
    }

    public void setCanExpressWarehouseCheck(boolean canExpressWarehouseCheck) {
        this.canExpressWarehouseCheck = canExpressWarehouseCheck;
    }

    public boolean isCanExpressOffShelf() {
        return canExpressOffShelf;
    }

    public void setCanExpressOffShelf(boolean canExpressOffShelf) {
        this.canExpressOffShelf = canExpressOffShelf;
    }

    public boolean isCanExpressReturnOffShelf() {
        return canExpressReturnOffShelf;
    }

    public void setCanExpressReturnOffShelf(boolean canExpressReturnOffShelf) {
        this.canExpressReturnOffShelf = canExpressReturnOffShelf;
    }

    public boolean isCanExpressDump() {
        return canExpressDump;
    }

    public void setCanExpressDump(boolean canExpressDump) {
        this.canExpressDump = canExpressDump;
    }

    public boolean isCanExpressPacking() {
        return canExpressPacking;
    }

    public void setCanExpressPacking(boolean canExpressPacking) {
        this.canExpressPacking = canExpressPacking;
    }

    public boolean isCanPackBoxQuery() {
        return canPackBoxQuery;
    }

    public void setCanPackBoxQuery(boolean canPackBoxQuery) {
        this.canPackBoxQuery = canPackBoxQuery;
    }

    public boolean isCanPackBoxShip() {
        return canPackBoxShip;
    }

    public void setCanPackBoxShip(boolean canPackBoxShip) {
        this.canPackBoxShip = canPackBoxShip;
    }

    public boolean isCanPhotoSignIn() {
        return canPhotoSignIn;
    }

    public void setCanPhotoSignIn(boolean canPhotoSignIn) {
        this.canPhotoSignIn = canPhotoSignIn;
    }

    public boolean isCanPhotoCheckIn() {
        return canPhotoCheckIn;
    }

    public void setCanPhotoCheckIn(boolean canPhotoCheckIn) {
        this.canPhotoCheckIn = canPhotoCheckIn;
    }

    public boolean isCanPhotoIssue() {
        return canPhotoIssue;
    }

    public void setCanPhotoIssue(boolean canPhotoIssue) {
        this.canPhotoIssue = canPhotoIssue;
    }

    public boolean isCanPhotoPack() {
        return canPhotoPack;
    }

    public void setCanPhotoPack(boolean canPhotoPack) {
        this.canPhotoPack = canPhotoPack;
    }

    public boolean isCanPhotoReturn() {
        return canPhotoReturn;
    }

    public void setCanPhotoReturn(boolean canPhotoReturn) {
        this.canPhotoReturn = canPhotoReturn;
    }

    public boolean isCanPhotoShip() {
        return canPhotoShip;
    }

    public void setCanPhotoShip(boolean canPhotoShip) {
        this.canPhotoShip = canPhotoShip;
    }

    public boolean isCanPhoto() {
        return canPhoto;
    }

    public void setCanPhoto(boolean canPhoto) {
        this.canPhoto = canPhoto;
    }
}
