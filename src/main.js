import './style.css'
import Phaser from 'phaser'

// 0
// var config = {
//   type: Phaser.WEBGL,
//   width: 800,
//   height: 600,
//   canvas: gameCanvas,
//   physics: {
//     default: 'arcade',
//     arcade: {
//       gravity: { y: 300 },
//       debug: false
//     }
//   },
//   scene: {
//     preload: preload,
//     create: create,
//     update: update
//   }
// };
// 0

//~ 3
var player;
//~ 3

// ?5
var stars;
//? 5

//^ 7
var bombs;
//^ 7

//todo 2
var platforms;
//todo 2

//! 4
var cursors;
//! 4

//& 6
var score = 0;
var scoreText;
//& 6

//^ 7
var gameOver = false;
//^ 7

// 0
// var game = new Phaser.Game(config);
// 0

function preload() {
  // 1
  // this.load.image('sky', 'assets/sky.png');
  // this.load.image('ground', 'assets/platform.png');
  // this.load.image('star', 'assets/star.png');
  // this.load.image('bomb', 'assets/bomb.png');
  // this.load.spritesheet('dude', 'assets/dude.png', { frameWidth: 32, frameHeight: 48 });
  // 1
}

function create() {
  //  A simple background for our game
  // 1
  // this.add.image(400, 300, 'sky');
  //this.add.image(400, 300, 'star');
  // 1

  //todo 2
  // //  The platforms group contains the ground and the 2 ledges we can jump on
  // platforms = this.physics.add.staticGroup();

  // //  Here we create the ground.
  // //  Scale it to fit the width of the game (the original sprite is 400x32 in size)
  // platforms.create(400, 568, 'ground').setScale(2).refreshBody();

  // //  Now let's create some ledges
  // platforms.create(600, 400, 'ground');
  // platforms.create(50, 250, 'ground');
  // platforms.create(750, 220, 'ground');
  //todo 2

  //~ 3
  // // The player and its settings
  // player = this.physics.add.sprite(100, 450, 'dude');

  // //  Player physics properties. Give the little guy a slight bounce.
  // player.setBounce(0.2);
  // player.setCollideWorldBounds(true);

  // //  Our player animations, turning, walking left and walking right.
  // this.anims.create({
  //   key: 'left',
  //   frames: this.anims.generateFrameNumbers('dude', { start: 0, end: 3 }),
  //   frameRate: 10,
  //   repeat: -1
  // });

  // this.anims.create({
  //   key: 'turn',
  //   frames: [{ key: 'dude', frame: 4 }],
  //   frameRate: 20
  // });

  // this.anims.create({
  //   key: 'right',
  //   frames: this.anims.generateFrameNumbers('dude', { start: 5, end: 8 }),
  //   frameRate: 10,
  //   repeat: -1
  // });
  //~ 3

  //! 4
  // //  Input Events
  // cursors = this.input.keyboard.createCursorKeys();
  //! 4

  //? 5
  // //  Some stars to collect, 12 in total, evenly spaced 70 pixels apart along the x axis
  // stars = this.physics.add.group({
  //   key: 'star',
  //   repeat: 11,
  //   setXY: { x: 12, y: 0, stepX: 70 }
  // });

  // stars.children.iterate(function (child) {

  //   //  Give each star a slightly different bounce
  //   child.setBounceY(Phaser.Math.FloatBetween(0.4, 0.8));

  // });
  //? 5

  //^ 7
  // bombs = this.physics.add.group();
  //^ 7

  //  The score
  //& 6
  // scoreText = this.add.text(16, 16, 'score: 0', { fontSize: '32px', fill: '#000' });
  //& 6

  //  Collide the player and the stars with the platforms
  //~ 3
  // this.physics.add.collider(player, platforms);
  //~ 3

  //? 5
  // this.physics.add.collider(stars, platforms);
  // this.physics.add.overlap(player, stars, collectStar, null, this);
  //? 5

  //^ 7
  // this.physics.add.collider(bombs, platforms);
  // this.physics.add.collider(player, bombs, hitBomb, null, this);
  //^ 7

  // Add rectangles for mobile controls
  //! 4
  // const leftRect = this.add.rectangle(100, 500, 100, 100, 0x0000ff, 0.5).setInteractive();
  // const rightRect = this.add.rectangle(300, 500, 100, 100, 0x0000ff, 0.5).setInteractive();
  // const jumpRect = this.add.rectangle(700, 500, 100, 100, 0x00ff00, 0.5).setInteractive();

  // // Add text labels to the rectangles
  // this.add.text(75, 480, 'Left', { fontSize: '20px', fill: '#ffffff' });
  // this.add.text(275, 480, 'Right', { fontSize: '20px', fill: '#ffffff' });
  // this.add.text(675, 480, 'Up', { fontSize: '20px', fill: '#ffffff' });

  // // Set up touch/click events for the rectangles
  // leftRect.on('pointerdown', () => cursors.left.isDown = true);
  // leftRect.on('pointerup', () => cursors.left.isDown = false);

  // rightRect.on('pointerdown', () => cursors.right.isDown = true);
  // rightRect.on('pointerup', () => cursors.right.isDown = false);

  // jumpRect.on('pointerdown', () => cursors.up.isDown = true);
  // jumpRect.on('pointerup', () => cursors.up.isDown = false);
  //! 4
}

function update() {
  //^ 7
  // if (gameOver) {
  //   return;
  // }
  //^ 7

  //! 4
  // if (cursors.left.isDown) {
  //   player.setVelocityX(-160);

  //   player.anims.play('left', true);
  // }
  // else if (cursors.right.isDown) {
  //   player.setVelocityX(160);

  //   player.anims.play('right', true);
  // }
  // else {
  //   player.setVelocityX(0);

  //   player.anims.play('turn');
  // }

  // if (cursors.up.isDown && player.body.touching.down) {
  //   player.setVelocityY(-330);
  // }
  //! 4

}

function collectStar(player, star) {
  //? 5
  // star.disableBody(true, true);
  //? 5

  //  Add and update the score
  //& 6
  // score += 10;
  // scoreText.setText('Score: ' + score);
  //& 6

  //^ 7
  // if (stars.countActive(true) === 0) {
  //   //  A new batch of stars to collect
  //   stars.children.iterate(function (child) {

  //     child.enableBody(true, child.x, 0, true, true);

  //   });

  //   var x = (player.x < 400) ? Phaser.Math.Between(400, 800) : Phaser.Math.Between(0, 400);

  //   var bomb = bombs.create(x, 16, 'bomb');
  //   bomb.setBounce(1);
  //   bomb.setCollideWorldBounds(true);
  //   bomb.setVelocity(Phaser.Math.Between(-200, 200), 20);
  //   bomb.allowGravity = false;

  // }
  //^ 7

}
//^ 7
// function hitBomb(player, bomb) {
//   this.physics.pause();

//   player.setTint(0xff0000);

//   player.anims.play('turn');

//   gameOver = true;
// }
//^ 7
